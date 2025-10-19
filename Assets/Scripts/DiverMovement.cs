using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class DiverMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float swimSpeed = 1f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float surfaceY = 32.5f;

    [Header("Animator")]
    private Animator animator;

    [SerializeField] private OxygenSystem oxygenSystem;
    private Rigidbody rb;
    private Vector3 input;
    private Vector3 currentVelocity;
    private bool isUnderwater = true;
    private bool canMove = true;

    private RescueAnimal currentAnimal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnterUnderwaterMode();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (!canMove) return;
        input.x = Input.GetAxis("Horizontal");
        if (isUnderwater)
            input.y = Input.GetAxis("Vertical");
        else
            input.y = 0f;

        input = input.normalized;
        CheckSurfaceTransition();

        animator.SetFloat("Horizontal", Mathf.Abs(input.x));
        if (Mathf.Abs(input.x) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (input.x > 0f ? 1f : -1f);
            transform.localScale = s;
        }
        animator.SetFloat("Down", Mathf.Abs(input.y));
    }

    void FixedUpdate()
    {
        if (!canMove) return;
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

        if (oxygenSystem != null)
            oxygenSystem.SetSafeZone(false);
    }

    void EnterLandMode()
    {
        isUnderwater = false;
        rb.useGravity = true;
        rb.linearDamping = 0f;
        Debug.Log("Tierra");

        if (oxygenSystem != null)
            oxygenSystem.SetSafeZone(true);
    }

    public void TeleportToBase(Vector3 basePosition)
    {
        transform.position = basePosition;
        canMove = false;
        rb.linearVelocity = Vector3.zero;
        Debug.Log("Jugador en Base");
    }
    public void TeleportToWater(Vector3 waterPosition)
    {
        transform.position = waterPosition;
        canMove = true;
        Debug.Log("Jugador en Agua");
    }
    public void EnableMovement(bool state)
    {
        canMove = state;
    }

    // método para asignar el animal rescatado desde otro script
    public void SetRescuedAnimal(RescueAnimal animal)
    {
        currentAnimal = animal;
    }
    public void SetSwimSpeed(float newSpeed)
    {
        swimSpeed = newSpeed;
        Debug.Log($"Nueva velocidad de nado: {swimSpeed}");
    }
    public float GetSwimSpeed()
    {
        return swimSpeed;
    }
}
