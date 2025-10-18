using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiverMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float surfaceY = 32.5f;

 

    private Rigidbody rb;
    private Vector3 input;
    private Vector3 currentVelocity;
    private bool isUnderwater = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnterUnderwaterMode();
        
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");

        if (isUnderwater)
            input.y = Input.GetAxis("Vertical");
        else
            input.y = 0f;

        input = input.normalized;

        CheckSurfaceTransition();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float speed = isUnderwater ? swimSpeed : walkSpeed;

        Vector3 targetVelocity = input * speed;

        if (!isUnderwater)
            targetVelocity.y = rb.linearVelocity.y;

        currentVelocity = Vector3.Lerp(
            rb.linearVelocity,
            targetVelocity,
            Time.fixedDeltaTime * (input.magnitude > 0 ? acceleration : deceleration)
        );

        rb.linearVelocity = currentVelocity;
    }

    void CheckSurfaceTransition()
    {
        if (isUnderwater && transform.position.y >= surfaceY)
        {
            EnterLandMode();
        }
        else if (!isUnderwater && transform.position.y < surfaceY)
        {
            EnterUnderwaterMode();
        }
    }

    void EnterUnderwaterMode()
    {
        isUnderwater = true;
        rb.useGravity = false;
        rb.linearDamping = 2f;
        Debug.Log("Agua");
    }

    void EnterLandMode()
    {
        isUnderwater = false;
        rb.useGravity = true;
        rb.linearDamping = 0f;
        Debug.Log("Tierra");
    }


}
