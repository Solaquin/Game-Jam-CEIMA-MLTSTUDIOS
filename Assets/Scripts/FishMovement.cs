using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FishMovement : MonoBehaviour
{
    public Transform player;

    [Header("Velocidades")]
    public float speed = 2f;
    public float speedAfter = 6f;

    [Header("Detección")]
    public float detectionRadius = 3.5f;
    public bool respondToPlayer = true; // false en Z ≠ 0

    [Header("Alert timing")]
    [Tooltip("Tiempo que permanece en alerta (acelerado) después de detectar al player.")]
    public float alertDuration = 1.5f;
    [Tooltip("Tiempo de espera tras salir de alerta antes de poder volver a entrar.")]
    public float alertRearmDelay = 0.75f;

    [Header("Colisión / Anti-pegarse")]
    public float pushOff = 0.02f;

    private Rigidbody rb;
    private Vector3 dirXY;
    private bool alert = false;
    private float z0;

    // temporizadores de alerta
    private float alertUntil = -1f;
    private float nextAlertAllowedTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        Vector2 r = Random.insideUnitCircle.normalized;
        if (r.sqrMagnitude < 1e-4f) r = Vector2.right;
        dirXY = new Vector3(r.x, r.y, 0f);

        z0 = transform.position.z;
    }

    public void SetPlayer(Transform p) => player = p;

    public void SetDepthAndBehavior(float z, bool respond)
    {
        z0 = z;
        respondToPlayer = respond;
        var p = transform.position; p.z = z0; transform.position = p;
    }

    void Update()
    {
        float now = Time.time;

        // Salir de alerta cuando se cumpla la duración
        if (alert && now >= alertUntil)
        {
            alert = false;
            // (ya quedó programado nextAlertAllowedTime cuando entró a alerta)
        }

        // Entrar a alerta si corresponde (solo si rearmado cumplido)
        if (!alert && respondToPlayer && player != null && now >= nextAlertAllowedTime)
        {
            Vector2 toPlayer = (Vector2)(player.position - transform.position);
            if (toPlayer.magnitude <= detectionRadius)
            {
                alert = true;
                alertUntil = now + alertDuration;
                nextAlertAllowedTime = alertUntil + alertRearmDelay;

                Vector2 away = -toPlayer;
                if (away.sqrMagnitude > 1e-6f)
                    dirXY = new Vector3(away.x, away.y, 0f).normalized;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 velXY = dirXY * (alert ? speedAfter : speed);
        velXY.z = 0f;
        rb.linearVelocity = velXY;

        if (Mathf.Abs(rb.position.z - z0) > 1e-4f)
            rb.position = new Vector3(rb.position.x, rb.position.y, z0);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.contactCount == 0) return;

        Vector3 n = c.GetContact(0).normal;

        // Rebote (reflect) como antes
        dirXY = Vector3.Reflect(dirXY, n);
        dirXY.z = 0f;
        if (dirXY.sqrMagnitude < 1e-6f) dirXY = -new Vector3(n.x, n.y, 0f);
        dirXY = dirXY.normalized;

        // Empujoncito fuera de la pared y re-aplicar velocidad
        rb.position += new Vector3(n.x, n.y, 0f) * pushOff;
        rb.linearVelocity = dirXY * (alert ? speedAfter : speed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
