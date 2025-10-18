using UnityEngine;

public class VacuumSystem : MonoBehaviour
{
    [Header("Vacuum Settings")]
    [SerializeField] private float suctionRadius = 5f;
    [SerializeField] private float suctionForce = 10f;
    [SerializeField] private float absorbDistance = 1f;
    [SerializeField] private float suctionAngle = 45f;
    [SerializeField] private Transform suctionPoint;
    [SerializeField] private LayerMask collectibleLayer;

    private bool isSucking = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (suctionPoint == null)
            suctionPoint = transform;
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

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(suctionPoint.position);

        Vector2 direction = (mouseScreenPos - playerScreenPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        suctionPoint.rotation = Quaternion.Euler(0, 0, angle);
    }



    void SuctionLogic()
    {
        Collider[] hits = Physics.OverlapSphere(suctionPoint.position, suctionRadius, collectibleLayer);

        foreach (var hit in hits)
        {
            if (IsInCone(hit.transform.position))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb == null) continue;

                Vector3 direction = (suctionPoint.position - rb.position).normalized;
                rb.AddForce(direction * suctionForce, ForceMode.Acceleration);

                float distance = Vector3.Distance(rb.position, suctionPoint.position);
                if (distance < absorbDistance)
                {
                    Debug.Log($"Absorbido: {hit.gameObject.name}");
                    Destroy(hit.gameObject);
                }
            }
        }
    }

    bool IsInCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - suctionPoint.position).normalized;
        Vector3 coneDirection = suctionPoint.right;

        float angleToTarget = Vector3.Angle(coneDirection, directionToTarget);
        return angleToTarget <= suctionAngle * 0.5f;
    }

    void OnDrawGizmosSelected()
    {
        if (suctionPoint == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(suctionPoint.position, suctionRadius);

        Gizmos.color = Color.cyan;
        DrawConeGizmo();
    }

    void DrawConeGizmo()
    {
        Vector3 coneDirection = suctionPoint.right;
        float halfAngle = suctionAngle * 0.5f;

        Vector3 leftBound = Quaternion.Euler(0, 0, halfAngle) * coneDirection * suctionRadius;
        Vector3 rightBound = Quaternion.Euler(0, 0, -halfAngle) * coneDirection * suctionRadius;

        Gizmos.DrawLine(suctionPoint.position, suctionPoint.position + leftBound);
        Gizmos.DrawLine(suctionPoint.position, suctionPoint.position + rightBound);
        Gizmos.DrawLine(suctionPoint.position + leftBound, suctionPoint.position + rightBound);
    }
}