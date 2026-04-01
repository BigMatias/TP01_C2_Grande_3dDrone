using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDataSO playerDataSO;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Transform playerPos;
    [Header("Combat Settings")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask hitLayers;
    [Header("Visual Effects")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserDuration = 0.05f;
    [Header("Laser Sight Settings")]
    [SerializeField] private LineRenderer laserSight;
    [SerializeField] private Color laserAimColor = Color.red;

    private Rigidbody rb;
    private HealthSystem healthSystem;
    private Ray ray;
    private bool isLaserSightActive = false;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        ChangePerspective();
        Shoot();
        UpdateRay();

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
    }

    private void FixedUpdate()
    {
        Movement();
        Rotate();        
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

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Vector3 endPoint;
            laserLine.SetPosition(0, playerPos.position);

            if (Physics.Raycast(ray, out hit, shootRange, hitLayers))
            {
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
        transform.Rotate(angle);
    }

    public float CurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    private void OnCollisionEnter(Collision other)
    {
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

}