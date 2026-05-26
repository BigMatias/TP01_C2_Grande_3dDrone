using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // Warning: [Baja] - Campo bulletLifeTime serializado pero nunca usado en el script.
    [SerializeField] private float bulletLifeTime = 2f;

    private PlayerController playerController;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float height;

    private float time;
    private float duration;

    private void Awake()
    {
        // Warning: [Alta] - GameObject.Find("Player") en cada bala spawneada del pool. Costoso y frágil ante renombres.
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

        // Bug: [Alta] - Si speed llega como 0 (defecto en el SO o asignación olvidada), duration = infinita, y la bala nunca llega al destino: queda en t=0 para siempre y nunca vuelve al pool → fuga de objetos.
        duration = distance / speed;

        time = 0f;
    }

    // Warning: [Media] - Si la bala impacta una pared u obstáculo neutro, no hay lógica de retorno al pool: la bala sigue volando hasta cerrar la curva. Hay que devolverla también en colisión genérica.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Layers.Enemy || other.gameObject.layer == (int)Layers.Civilian)
        {
            playerController.ReturnBulletToPool(gameObject);

            // Bug: [Alta] - GetComponent puede devolver null. Sin validación → NullReferenceException si el enemigo perdió su HealthSystem o se asignó a un objeto sin el componente.
            HealthSystem targetHealth = other.gameObject.GetComponent<HealthSystem>();
            // Suggestion: [Media] - Daño 100 hardcodeado. playerDataSO.SecondaryShotDamage existe específicamente para esto y queda ignorado.
            targetHealth.DoDamage(100);
        }
    }
}