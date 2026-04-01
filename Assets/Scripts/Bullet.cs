using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;

    private float speed; 
    private float distance;
    private float traveled;

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)Layers.Player)
        {
            Debug.Log("hola");
            HealthSystem targetHealth = collision.gameObject.GetComponent<HealthSystem>();
            targetHealth.DoDamage(5);
            gameObject.SetActive(false);
        }
    }
}