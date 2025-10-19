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

    [Header("Audio (solo ambiente)")]
    [Tooltip("Loop cuando estás bajo el agua")]
    [SerializeField] private AudioClip WaterAmbienceLoop;
    [Tooltip("Loop cuando estás arriba/en base")]
    [SerializeField] private AudioClip LandAmbienceLoop;
    [Range(0f, 1f)][SerializeField] private float ambienceVolume = 0.8f;

    private AudioSource ambientSource;

    [Header("Animator")]
    private Animator animator;

    [SerializeField] private OxygenSystem oxygenSystem;
    private VacuumSystem vacuumSystem;
    private Rigidbody rb;
    private Vector3 input;
    private Vector3 currentVelocity;
    private bool isUnderwater = true;
    private bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        vacuumSystem = GetComponent<VacuumSystem>();

        // AudioSource dedicado al ambiente
        ambientSource = GetComponent<AudioSource>();
        if (ambientSource == null) ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        ambientSource.volume = ambienceVolume;
        ambientSource.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        EnterUnderwaterMode(); // estado inicial
    }

    void Update()
    {
        CheckSurfaceTransition();

        if (!canMove) return;

        input.x = Input.GetAxis("Horizontal");
        input.y = isUnderwater ? Input.GetAxis("Vertical") : 0f;
        input = input.normalized;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", Mathf.Abs(input.x));
            animator.SetFloat("Down", Mathf.Abs(input.y));
        }

        if (Mathf.Abs(input.x) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (input.x > 0f ? 1f : -1f);
            transform.localScale = s;
        }
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

        StartAmbience(WaterAmbienceLoop);

        if (oxygenSystem != null)
            oxygenSystem.SetSafeZone(false);
    }

    void EnterLandMode()
    {
        isUnderwater = false;
        rb.useGravity = true;
        rb.linearDamping = 0f;

        StartAmbience(LandAmbienceLoop);

        if (oxygenSystem != null)
            oxygenSystem.SetSafeZone(true);
    }

    public void TeleportToBase(Vector3 basePosition)
    {
        transform.position = basePosition;
        if (vacuumSystem != null) vacuumSystem.enabled = false;
        canMove = false;
        rb.linearVelocity = Vector3.zero;
    }

    public void TeleportToWater(Vector3 waterPosition)
    {
        transform.position = waterPosition;
        if (vacuumSystem != null) vacuumSystem.enabled = true;
        canMove = true;
    }

    public void EnableMovement(bool state) => canMove = state;
    public void SetSwimSpeed(float newSpeed) => swimSpeed = newSpeed;
    public float GetSwimSpeed() => swimSpeed;

    // ——— Audio ambiente ———
    private void StartAmbience(AudioClip clip)
    {
        if (clip == null)
        {
            ambientSource.Stop();
            ambientSource.clip = null;
            return;
        }

        if (ambientSource.clip == clip)
        {
            if (!ambientSource.isPlaying) ambientSource.Play();
            return;
        }

        ambientSource.Stop();
        ambientSource.clip = clip;
        ambientSource.volume = ambienceVolume;
        ambientSource.Play();
    }
}
