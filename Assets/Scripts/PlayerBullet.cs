using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float bulletLifeTime = 2f;

    private PlayerController playerController;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float height;

    private float time;
    private float duration;

    private void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        time += Time.deltaTime;

        float t = time / duration;

        if (t >= 1f)
        {
            playerController.ReturnBulletToPool(gameObject);
            return;
        }

        Vector3 pos = Vector3.Lerp(startPoint, endPoint, t);

        float yOffset = height * 4f * (t * (1 - t));

        pos.y += yOffset;

        transform.position = pos;
    }

    public void Init(Vector3 start, Vector3 end, float arcHeight, float speed)
    {
        startPoint = start;
        endPoint = end;
        height = arcHeight;

        float distance = Vector3.Distance(start, end);

        duration = distance / speed; 

        time = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Layers.Enemy || other.gameObject.layer == (int)Layers.Civilian)
        {
            playerController.ReturnBulletToPool(gameObject);

            HealthSystem targetHealth = other.gameObject.GetComponent<HealthSystem>();
            targetHealth.DoDamage(100);
        }
    }
}