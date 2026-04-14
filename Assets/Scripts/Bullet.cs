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
            HealthSystem targetHealth = collision.gameObject.GetComponent<HealthSystem>();
            targetHealth.DoDamage(citizenDataSO.EnemyBulletDamage);
        }
        spawner.ReturnBulletToPool(gameObject);
        
    }
}