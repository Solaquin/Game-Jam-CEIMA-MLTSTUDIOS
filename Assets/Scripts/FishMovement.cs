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
    

    private Rigidbody rb;
    private Vector2 direction;
    private bool alert = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        direction = Random.insideUnitCircle.normalized;
        if (direction == Vector2.zero) direction = Vector2.right;
    }

    void Update()
    {
        if (!alert && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist < detectionradius)
            {
                Debug.Log("Alerta");
                alert = true;
                direction = -direction.normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * (alert ? speedAfter : speed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( transform.position, detectionradius);
    }


}
