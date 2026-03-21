using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        Movement();
        Rotate();        
    }
    private void Movement()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movement += Time.fixedDeltaTime * speed;

        rb.AddForce(movement);
    }

    private void Rotate()
    {
        Vector3 angle = new Vector3(0, Input.GetAxis("Mouse X"));
        transform.Rotate(angle);
    }

    private void OnCollisionEnter(Collision other)
    {

    }
}