using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerDataSO playerDataSO;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;

    private Rigidbody rb;
    private HealthSystem healthSystem;
    
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

}