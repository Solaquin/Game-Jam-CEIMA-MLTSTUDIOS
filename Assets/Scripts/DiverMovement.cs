using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiverMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private TargetType targetType = TargetType.Player;

    private Rigidbody rb;
    private Vector2 input;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        input = input.normalized;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
       
        Vector2 targetVelocity = input * swimSpeed;

        currentVelocity = Vector2.Lerp(
            currentVelocity,
            targetVelocity,
            Time.fixedDeltaTime * (input.magnitude > 0 ? acceleration : deceleration)
        );

        rb.linearVelocity = currentVelocity;
    }
}
