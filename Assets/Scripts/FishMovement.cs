using Unity.VisualScripting;
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
    public bool respondToPlayer = true; 

    [Header("Alert timing")]
    [Tooltip("Tiempo que permanece en alerta (acelerado) después de detectar al player.")]
    public float alertDuration = 1.5f;
    [Tooltip("Tiempo de espera tras salir de alerta antes de poder volver a entrar.")]
    public float alertRearmDelay = 0.75f;

    [Header("Colisión / Anti-pegarse")]
    public float pushOff = 0.02f;

    [Header("Limites Y")]
    public bool limitY = true;
    public float yMin = 0f;
    public float yMax = 21f;
    public bool bounceOnLimits = true;

    [Header("Orientación del sprite")]
    [Tooltip("Hace que el pez apunte en la dirección en que se mueve")]
    public bool faceMovement = true;
    [Tooltip("Suavizado del giro (grados/seg). 0 = giro instantáneo")]
    public float turnSmoothing = 720f; // prueba 360–1080
    [Tooltip("Velocidad mínima para considerar un cambio de orientación")]
    public float minFaceSpeed = 0.05f;

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

    public void SetYBounds(float min, float max, bool bounce = true)
    {
        limitY = true;
        yMin = min;
        yMax = max;
        bounceOnLimits = bounce;
    }

    void Update()
    {
        float now = Time.time;

        if (alert && now >= alertUntil)
        {
            alert = false;

        }

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

        if (limitY)
        {
            var pos = rb.position;
            bool bounced = false;

            if (pos.y > yMax)
            {
                pos.y = yMax;
                if (bounceOnLimits)
                {
                    dirXY.y = -Mathf.Abs(dirXY.y);
                    bounced = true;
                }
            }
            else if(pos.y < yMin)
            {
                pos.y = yMin;
                if(bounceOnLimits)
                {
                    dirXY.y = Mathf.Abs(dirXY.y);
                    bounced = true;
                }
            }
            if (bounced)
                rb.linearVelocity = dirXY * (alert ? speedAfter : speed);

            rb.position = new Vector3(pos.x, pos.y, z0);
        }
        if (faceMovement)
        {
            Vector3 v = rb.linearVelocity;
            v.z = 0f;

            if (v.sqrMagnitude > minFaceSpeed * minFaceSpeed)
            {
  
                float targetAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

                float current = transform.eulerAngles.z;
                float newZ = Mathf.MoveTowardsAngle(current, targetAngle, turnSmoothing * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, newZ);
            }
        }
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
