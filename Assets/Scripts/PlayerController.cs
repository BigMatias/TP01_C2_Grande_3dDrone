using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDataSO playerDataSO;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform shootPoint;
    [Header("Combat Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask hitLayers;
    [Header("Visual Effects")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserDuration = 0.05f;
    [Header("Laser Sight Settings")]
    [SerializeField] private LineRenderer laserSight;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    private Transform playerBullets;
    private Rigidbody rb;
    private HealthSystem healthSystem;
    private Ray ray;
    private bool isLaserSightActive = false;

    public static event Action onPlayerDied;
    public static event Action onPlayerShootM1;
    public static event Action onPlayerShootM2;
    public static event Action onPlayerHurt;

    // Warning: [Media] - Singleton público sin encapsular. Cualquier script puede sobrescribirlo. Debería ser { get; private set; }.
    public static PlayerController Instance;
    private float shootCdAux;

    private void Awake()
    {
        // Bug: [Media] - No se valida si ya hay otra instancia.
        Instance  = this;
        // Warning: [Alta] - "PlayerBullets" es un magic string acoplado al nombre del hijo en jerarquía. Si alguien renombra el GameObject, esto devuelve null y rompe InstantiateBullets() silenciosamente.
        playerBullets = transform.Find("PlayerBullets");
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDie += HealthSystem_onDie;
        // Error: [Baja] - Doble punto y coma al final (";;").
        healthSystem.onDamageDealt += HealthSystem_onDamageDealt; ;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Warning: [Baja] - bulletPool ya se inicializa en la declaración del campo (línea 25).
        bulletPool = new Queue<GameObject>();
        InstantiateBullets();
    }

    private void Update()
    {
        ChangePerspective();
        UpdateRay();
        HandleRotation();
        if (Input.GetKeyDown(KeyCode.C))
        {
            isLaserSightActive = !isLaserSightActive; 

            if (!isLaserSightActive)
            {
                laserSight.enabled = false;
            }
        }

        if (isLaserSightActive)
        {
            UpdateLaserSight();
        }

        Shoot();
        SecondaryShoot();
    }

    private void FixedUpdate()
    {
        Movement();
        // Bug: [Alta] - Rotate() llama transform.Rotate sobre un objeto con Rigidbody y dentro de FixedUpdate. Esto pelea con el motor físico, produce jittering y puede provocar NaN en colisiones. Lo correcto: rb.MoveRotation(...).
        Rotate();
    }

    private void HealthSystem_onDie()
    {
        onPlayerDied?.Invoke();
    }
    private void HealthSystem_onDamageDealt()
    {
        onPlayerHurt?.Invoke();
    }
    private void InstantiateBullets()
    {
        for (int i = 0; i < playerDataSO.BulletQuantityInstantiate; i++)
        {
            GameObject obj = Instantiate(bullet, playerBullets);
            bulletPool.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    private void ChangePerspective()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (thirdPersonCamera.gameObject.activeSelf)
            {
                thirdPersonCamera.gameObject.SetActive(false);
                firstPersonCamera.gameObject.SetActive(true);
            }
            else if (firstPersonCamera.gameObject.activeSelf)
            {
                firstPersonCamera.gameObject.SetActive(false);
                thirdPersonCamera.gameObject.SetActive(true);
            }
        }
    }

    private void Movement()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 inputDir = (forward * v + right * h).normalized;

        if (inputDir != Vector3.zero)
        {
            rb.AddForce(inputDir * playerDataSO.Acceleration, ForceMode.Acceleration);
        }


        if (rb.linearVelocity.magnitude > playerDataSO.MaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * playerDataSO.MaxSpeed;
        }

        if (Input.GetKey(playerDataSO.GoDown))
        {
            rb.AddForce(Vector3.down * playerDataSO.Acceleration, ForceMode.Acceleration);
        }
        if (Input.GetKey(playerDataSO.GoUp))
        {
            rb.AddForce(Vector3.up * playerDataSO.Acceleration, ForceMode.Acceleration);
        }
    }
    private void HandleRotation()
    {
        float rotationInput = 0f;

        if (Input.GetKey(playerDataSO.RotateLeft))
            rotationInput = 1f;

        if (Input.GetKey(playerDataSO.RotateRight))
            rotationInput = -1f;

        transform.Rotate(0f, 0f, rotationInput * playerDataSO.RotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            onPlayerShootM1?.Invoke();
            RaycastHit hit;
            Vector3 endPoint;
            laserLine.SetPosition(0, playerPos.position);

            if (Physics.Raycast(ray, out hit, shootRange, hitLayers))
            {
                // Warning: [Baja] - Debug.Log activo en build → spam en consola y costo de GC por concatenación de string en cada disparo.
                Debug.Log("Impacto en: " + hit.collider.name);
                endPoint = hit.point;

                HealthSystem targetHealth = hit.collider.GetComponent<HealthSystem>();
                if (targetHealth != null)
                {
                    targetHealth.DoDamage(damage);
                }
                
            }
            else
            {
                endPoint = playerPos.position + (ray.direction * shootRange);
            }
            laserLine.SetPosition(1, endPoint);
            StartCoroutine(ShootEffectSequence());
        }
    }

    private void SecondaryShoot()
    {
        // Error: [Alta] - shootCdAux se decrementa pero NUNCA se compara antes de disparar. El cooldown está "escrito" pero no se aplica: se puede disparar M2 cada frame. playerDataSO.SecondaryShotCD queda desconectado.
        shootCdAux -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            onPlayerShootM2?.Invoke();

            // Warning: [Alta] - Camera.main internamente hace FindGameObjectWithTag("MainCamera"). Ejecutarlo en cada disparo es costoso; cachear en awake
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 50f;
            }

            // Bug: [Crítica] - Si bulletPool está vacío (más disparos que BulletQuantityInstantiate o balas todavía viajando), Dequeue() lanza InvalidOperationException. No hay TryDequeue ni expansión del pool.
            GameObject bullet = bulletPool.Dequeue();

            bullet.SetActive(true);

            // Warning: [Media] - GetComponent<PlayerBullet>() en cada disparo. Si las balas se reciclan, cachear el componente en InstantiateBullets() evita este costo.
            bullet.GetComponent<PlayerBullet>().Init(
                shootPoint.position,
                targetPoint,
                // Suggestion: [Baja] - Magic numbers (2f, 100f, 50f en este método). Deberían vivir como campos serializados o en el ScriptableObject.
                2f,
                playerDataSO.BulletSpeed
            );
        }
    }

    private void UpdateRay()
    {
        Camera activeCam = firstPersonCamera.gameObject.activeSelf ? firstPersonCamera : thirdPersonCamera;
        ray = activeCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    }

    private void UpdateLaserSight()
    {
        if (!laserSight.enabled) laserSight.enabled = true;

        RaycastHit hit;
        laserSight.SetPosition(0, playerPos.position);

        if (Physics.Raycast(ray, out hit, shootRange, hitLayers))
        {
            laserSight.SetPosition(1, hit.point);
        }
        else
        {
            laserSight.SetPosition(1, playerPos.position + (ray.direction * shootRange));
        }
    }

    private void Rotate()
    {
        Vector3 angle = new Vector3(playerDataSO.MouseSens * (Input.GetAxis("Mouse Y") * - 1), playerDataSO.MouseSens * Input.GetAxis("Mouse X"));
        // Bug: [Media] - transform.Rotate sobre un Rigidbody (debería ser rb.MoveRotation). Además HandleRotation() ya rota en Z desde Update: hay dos sistemas de rotación operando sobre el mismo transform, lo que produce conflictos visuales.
        transform.Rotate(angle);
    }

    public float CurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Warning: [Media] - Comparar layer con (int)Layers.Obstacles asume que el enum representa el ÍNDICE numérico de la capa en el Tag&Layer Manager. Funciona porque coincide pero es frágil ante reordenamientos. Mejor LayerMask serializado o LayerMask.NameToLayer.
        if (other.gameObject.layer == (int)Layers.Obstacles)
        {
            CrashedWithObstacle(other.relativeVelocity.magnitude);
        }
    }

    private void CrashedWithObstacle(float impactSpeed)
    {
        if (impactSpeed < 2f) return;
        if (impactSpeed <= playerDataSO.FirstSpeedThreshold)
        {
            healthSystem.DoDamage(playerDataSO.CrashDamage1);
            return;
        }
        else if (impactSpeed <= playerDataSO.SecondSpeedThreshold)
        {
            healthSystem.DoDamage(playerDataSO.CrashDamage2);
            return;
        }
        else
        {
            healthSystem.DoDamage(playerDataSO.CrashDamage3);
        }
    }

    private IEnumerator ShootEffectSequence()
    {
        laserLine.enabled = true; 
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false; 
    }
    public void ReturnBulletToPool(GameObject obj)
    {
        bulletPool.Enqueue(obj);
        obj.SetActive(false);
    }
}