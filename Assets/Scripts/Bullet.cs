using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private CitizenDataSO citizenDataSO;

    private CitizensSpawner spawner;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float speed; 
    private float distance;
    private float traveled;
    private float bulletLifeTimeAux;

    private void Awake()
    {
        // Warning: [Alta] - GameObject.Find por cada bala instanciada del pool. Recorre toda la jerarquía. Pasar el spawner por Init() o un Setup(spawner) es trivial y mucho más barato.
        // Bug: [Alta] - "Ciudadanos" es magic string (Como los number magic pero de string) . Si el GameObject se renombra o no existe, GetComponent sobre null lanza NullReferenceException y la bala queda inservible.
        spawner = GameObject.Find("Ciudadanos").GetComponent<CitizensSpawner>();
    }

    private void Start()
    {
        bulletLifeTimeAux = citizenDataSO.EnemyBulletLifeTime;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        traveled += step;

        float t = traveled / distance;

        if (t >= 1f)
        {
            transform.position = targetPos;
            // Error: [Crítica] - Contradicción de patrones: la clase usa pool (ReturnBulletToPool) PERO acá Destroy(gameObject).
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        bulletLifeTimeAux -= Time.deltaTime;

        if (bulletLifeTimeAux <= 0)
        {
            spawner.ReturnBulletToPool(gameObject);
            bulletLifeTimeAux = citizenDataSO.EnemyBulletLifeTime;
        }
    }

    public void Init(Vector3 start, Vector3 target, float speed)
    {
        startPos = start;
        targetPos = target;
        this.speed = speed;

        transform.position = startPos;

        Vector3 dir = (targetPos - startPos).normalized;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        distance = Vector3.Distance(startPos, targetPos);
        traveled = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)Layers.Player)
        {
            // Bug: [Alta] - Si Player no tiene HealthSystem (o se pierde por bug), targetHealth es null y DoDamage rompe con NullReferenceException. Falta if (targetHealth != null).
            HealthSystem targetHealth = collision.gameObject.GetComponent<HealthSystem>();
            targetHealth.DoDamage(citizenDataSO.EnemyBulletDamage);
        }
        // Error: [Media] - La bala vuelve al pool ante cualquier colisión (paredes, suelo, civiles, NPCs). Combinado con el Destroy() de Update, el pool se llena de referencias inválidas.
        spawner.ReturnBulletToPool(gameObject);

    }
}