using UnityEngine;

[RequireComponent(typeof(VacuumSystem))]
public class VacuumVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int lineSegments = 24;
    [SerializeField] private Gradient suctionColor;
    [SerializeField] private float lineWidth = 0.05f;

    private VacuumSystem vacuumSystem;
    private Transform suctionPoint;

    void Start()
    {
        vacuumSystem = GetComponent<VacuumSystem>();

        // Referencia al punto real de succión
        suctionPoint = GetPrivateField<Transform>(vacuumSystem, "suctionPoint");
        if (suctionPoint == null)
            suctionPoint = transform;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configurar el LineRenderer
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.colorGradient = suctionColor;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (vacuumSystem == null || suctionPoint == null)
            return;

        bool isSucking = vacuumSystem.IsSucking; // Getter público en VacuumSystem

        if (isSucking)
        {
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            DrawCone();
        }
        else
        {
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
        }
    }

    void DrawCone()
    {
        float radius = vacuumSystem.GetSuctionRadius();
        float angle = vacuumSystem.GetSuctionAngle();
        Vector3 origin = suctionPoint.position;
        Vector3 forward = suctionPoint.right; // Usa la dirección que sigue al mouse

        float halfAngle = angle * 0.5f;
        lineRenderer.positionCount = lineSegments + 2;

        // Primer punto: la boquilla
        lineRenderer.SetPosition(0, origin);

        // Dibujar los bordes del cono
        for (int i = 0; i <= lineSegments; i++)
        {
            float t = (float)i / lineSegments;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * forward;
            Vector3 point = origin + dir * radius;
            lineRenderer.SetPosition(i + 1, point);
        }
    }
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (T)field.GetValue(obj) : default;
    }
}
