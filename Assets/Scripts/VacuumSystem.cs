using UnityEngine;

public class VacuumSystem : MonoBehaviour
{
    [Header("Vacuum Settings")]
    [SerializeField] private float suctionRadius = 5f;
    [SerializeField] private float suctionForce = 10f;
    [SerializeField] private float absorbDistance = 1f;
    [SerializeField] private Transform suctionPoint; 
    [SerializeField] private LayerMask collectibleLayer;

    private bool isSucking = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0))
            isSucking = true;
        if (Input.GetMouseButtonUp(0))
            isSucking = false;
    }

    void FixedUpdate()
    {
        if (isSucking)
            SuctionLogic();
    }

    void RotateTowardsMouse()
    {
        if (mainCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMouse = mainCamera.ScreenToWorldPoint(mousePos);
        worldMouse.z = transform.position.z;

        Vector3 dir = worldMouse - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        suctionPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void SuctionLogic()
    {
        Collider[] hits = Physics.OverlapSphere(suctionPoint.position, suctionRadius);
        Debug.Log($"Detectados: {hits.Length}");

        foreach (var hit in hits)
        {
            Debug.Log($"? Detectado: {hit.gameObject.name}");
            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null)
            {
                Debug.Log($"Sin Rigidbody: {hit.name}");
                continue;
            }

            Vector3 direction = (suctionPoint.position - rb.position).normalized;
            rb.AddForce(direction * suctionForce, ForceMode.Acceleration);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (suctionPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(suctionPoint.position, suctionRadius);
    }
}
