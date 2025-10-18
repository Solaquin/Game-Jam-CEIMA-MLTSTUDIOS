using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FishMovement : MonoBehaviour
{

    public Transform player;
    [SerializeField]
    public float speed = 2f;
    public float speedAfter = 6f;

    public float detectionradius = 3.5f;

    public float empujon = 0.02f;

    private Rigidbody rb;
    private Vector3 direction;
    private bool alert = false;
    private float z0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        direction = Random.insideUnitCircle.normalized;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        direction = Random.insideUnitCircle.normalized;
        if (direction.sqrMagnitude < 1e-4f) direction = Vector2.right;
        z0 = transform.position.z;
    }

    void Update()
    {

        if (!alert && player != null)
        {
            Vector2 toPlayer = (player.position - transform.position);
            if (toPlayer.magnitude < detectionradius)
            {
                Debug.Log("Alerta");
                alert = true;
                direction = (-toPlayer).normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 velXY = new Vector3(direction.x, direction.y, 0) * (alert ? speedAfter : speed);
        rb.linearVelocity = velXY;
    }


    void OnCollisionEnter(Collision c)
    {
        if (c.contactCount == 0) return;

  
        Vector3 n3 = c.GetContact(0).normal;
        Vector2 n = new Vector2(n3.x, n3.y).normalized;
        if (n.sqrMagnitude < 1e-6f) { direction = -direction; }
        else
        {
            
            direction = Vector2.Reflect(direction, n).normalized;
        }

        Vector2 v = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        float into = Vector2.Dot(v, n); // <0 si se mete al muro
        if (into < 0f) v -= n * into;

        rb.position += new Vector3(n.x, n.y, 0f) * empujon;
        rb.linearVelocity = new Vector3(direction.x, direction.y, 0f) * (alert ? speedAfter : speed);
        alert = false;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( transform.position, detectionradius);
    }


}
