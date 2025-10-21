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

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip suctionLoopSound; 
    [SerializeField] private AudioClip collectSound;    
    [SerializeField] private float suctionVolume = 0.7f;
    [SerializeField] private float collectVolume = 1f;


    private BagSystem bagSystem;
    private bool isSucking = false;
    private Camera mainCamera;
    private bool wasSucking = false;
    public float CurrentSuctionRadius { get; private set; }
    public float CurrentSuctionAngle { get; private set; }
    void Start()
    {
        bagSystem = GetComponent<BagSystem>();
        mainCamera = Camera.main;

        if (suctionPoint == null)
            suctionPoint = transform;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.volume = suctionVolume;
    }

    void Update()
    {
        //Debug.Log($"Vacuum activo: {name}");

        //Debug.Log(GetSuctionRadius());
        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0))
            isSucking = true;
        if (Input.GetMouseButtonUp(0))
            isSucking = false;

        HandleSuctionSound();

    }

    void FixedUpdate()
    {
        if (isSucking)
            SuctionLogic();
        //Debug.Log($"Radio actual usado en física: {suctionRadius}");
    }
    public void HandleSuctionSound()
    {
        if (isSucking && !wasSucking)
        {
            // Empezar a aspirar
            if (suctionLoopSound != null && audioSource != null)
            {
                audioSource.clip = suctionLoopSound;
                audioSource.Play();
                Debug.Log("Sonido de aspiración iniciado");
            }
        }
        else if (!isSucking && wasSucking)
        {
            // Dejar de aspirar
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                Debug.Log("Sonido de aspiración detenido");
            }
        }

        wasSucking = isSucking;
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
            Debug.Log($"Detectado: {hit.name} en layer {LayerMask.LayerToName(hit.gameObject.layer)}");
            if (IsInCone(hit.transform.position))
            {
                Rigidbody rb = hit.attachedRigidbody;
                ScriptableObject collectibleItem = hit.gameObject.GetComponent<CollectibleItem>().itemData;

                if (collectibleItem == null) 
                if (rb == null) continue;

                Vector3 direction = (suctionPoint.position - rb.position).normalized;
                rb.AddForce(direction * suctionForce, ForceMode.Acceleration);

                float distance = Vector3.Distance(rb.position, suctionPoint.position);
                if (distance < absorbDistance && bagSystem.canAddNextItem(collectibleItem, 1))
                {
                    bagSystem.AddItem(collectibleItem, 1);
                    if (collectSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(collectSound, collectVolume);
                        Debug.Log("Sonido de recolección reproducido");
                    }
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

        // Dibuja el arco del círculo para mostrar el límite del radio completo
        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float t1 = -halfAngle + (i * (suctionAngle / segments));
            float t2 = -halfAngle + ((i + 1) * (suctionAngle / segments));

            Vector3 p1 = suctionPoint.position + Quaternion.Euler(0, 0, t1) * coneDirection * suctionRadius;
            Vector3 p2 = suctionPoint.position + Quaternion.Euler(0, 0, t2) * coneDirection * suctionRadius;

            Gizmos.DrawLine(p1, p2);
        }
    }

    public void SetSuctionRadius(float newRadius)
    {
        suctionRadius = newRadius;
        Debug.Log($"Radio de succión actualizado a: {suctionRadius}");
    }
    public void SetSuctionAngle(float newAngle)
    {
        suctionAngle = newAngle;
        Debug.Log($"Ángulo de succión actualizado a: {suctionAngle}");
    }

    public float GetSuctionRadius() => suctionRadius;
    public float GetSuctionAngle() => suctionAngle;
    public bool IsSucking => isSucking;
    public void SetIsSucking(bool _isSucking)
    {
        isSucking = _isSucking;
    }
    
}