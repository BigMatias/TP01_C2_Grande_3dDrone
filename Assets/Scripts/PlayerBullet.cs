using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private PlayerDataSO playerDataSO;
    [SerializeField] float bulletLifeTime;

    private PlayerController playerController;

    private float speed;
    private float bulletLifeTimeAux;
    private Vector3 direction;

    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        bulletLifeTimeAux -= Time.deltaTime;
        if (bulletLifeTimeAux <= 0)
        {
            playerController.ReturnBulletToPool(gameObject);
        }
    }

    public void Init(Vector3 start, Vector3 dir, float bulletSpeed)
    {
        transform.position = start;
        direction = dir.normalized;
        speed = bulletSpeed;
        bulletLifeTimeAux = bulletLifeTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)Layers.Enemy)
        {
            HealthSystem targetHealth = collision.gameObject.GetComponent<HealthSystem>();
            targetHealth.DoDamage(playerDataSO.SecondaryShotDamage);
        }
        playerController.ReturnBulletToPool(gameObject);

    }
}