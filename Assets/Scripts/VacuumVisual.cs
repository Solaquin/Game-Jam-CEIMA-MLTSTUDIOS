using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(VacuumSystem))]
public class VacuumVisual : MonoBehaviour
{
    [Header("Main Cone Settings")]
    [SerializeField] private LineRenderer coneRenderer;
    [SerializeField] private int lineSegments = 24;
    [SerializeField] private Gradient baseColor;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private Color pulseA = Color.cyan;
    [SerializeField] private Color pulseB = Color.blue;

    [Header("Flow Line Settings")]
    [SerializeField] private Gradient suctionLineColor;
    [SerializeField] private float suctionLineWidth = 0.025f;
    [SerializeField] private float suctionLineSpeed = 2f;
    [SerializeField] private float spawnInterval = 0.07f;
    [SerializeField] private int maxFlowLines = 10;

    private VacuumSystem vacuumSystem;
    private Transform suctionPoint;
    private List<GameObject> activeLines = new List<GameObject>();
    private bool spawningLines = false;

    void Start()
    {
        vacuumSystem = GetComponent<VacuumSystem>();
        suctionPoint = GetPrivateField<Transform>(vacuumSystem, "suctionPoint") ?? transform;

        if (coneRenderer == null)
            coneRenderer = gameObject.AddComponent<LineRenderer>();

        // Configuración del cono principal
        coneRenderer.useWorldSpace = true;
        coneRenderer.loop = true;
        coneRenderer.startWidth = lineWidth;
        coneRenderer.endWidth = lineWidth;
        coneRenderer.material = new Material(Shader.Find("Sprites/Default"));
        coneRenderer.colorGradient = baseColor;
        coneRenderer.enabled = false;
    }

    void Update()
    {
        if (vacuumSystem == null || suctionPoint == null)
            return;

        bool isSucking = vacuumSystem.IsSucking;

        if (isSucking)
        {
            if (!coneRenderer.enabled)
            {
                coneRenderer.enabled = true;
                StartCoroutine(SpawnSuctionLines());
            }
        }
        else
        {
            if (coneRenderer.enabled)
                coneRenderer.enabled = false;

            StopAllCoroutines();
            ClearFlowLines();
        }

        // Actualiza las líneas activas mientras el jugador se mueve
        UpdateFlowLines();
    }

    void DrawCone()
    {
        float radius = vacuumSystem.GetSuctionRadius();
        float angle = vacuumSystem.GetSuctionAngle();
        Vector3 origin = suctionPoint.position;
        Vector3 forward = suctionPoint.right;
        float halfAngle = angle * 0.5f;

        coneRenderer.positionCount = lineSegments + 2;
        coneRenderer.SetPosition(0, origin);

        for (int i = 0; i <= lineSegments; i++)
        {
            float t = (float)i / lineSegments;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * forward;
            coneRenderer.SetPosition(i + 1, origin + dir * radius);
        }
    }

    void AnimateConePulse()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        Color startColor = Color.Lerp(pulseA, pulseB, pulse);
        Color endColor = Color.Lerp(pulseB, pulseA, pulse);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.3f, 1f)
            }
        );
        coneRenderer.colorGradient = gradient;
    }

    IEnumerator SpawnSuctionLines()
    {
        spawningLines = true;

        while (vacuumSystem.IsSucking)
        {
            if (activeLines.Count < maxFlowLines)
            {
                GameObject lineObj = CreateFlowLine();
                activeLines.Add(lineObj);
                StartCoroutine(AnimateFlowLine(lineObj));
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        spawningLines = false;
    }

    GameObject CreateFlowLine()
    {
        GameObject line = new GameObject("SuctionFlowLine");
        var lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.colorGradient = suctionLineColor;
        lr.startWidth = suctionLineWidth;
        lr.endWidth = 0f;
        lr.positionCount = 2;
        return line;
    }

    IEnumerator AnimateFlowLine(GameObject lineObj)
    {
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        float t = 0f;
        float angle = Random.Range(-vacuumSystem.GetSuctionAngle() / 2f, vacuumSystem.GetSuctionAngle() / 2f);

        while (vacuumSystem.IsSucking)
        {
            if (suctionPoint == null) break;

            Vector3 dir = Quaternion.Euler(0, 0, angle) * suctionPoint.right;
            Vector3 start = suctionPoint.position + dir * vacuumSystem.GetSuctionRadius();
            Vector3 end = suctionPoint.position;

            t += Time.deltaTime * suctionLineSpeed;
            Vector3 mid = Vector3.Lerp(start, end, Mathf.Clamp01(t));

            lr.SetPosition(0, start);
            lr.SetPosition(1, mid);

            if (t >= 1f)
            {
                t = 0f;
                angle = Random.Range(-vacuumSystem.GetSuctionAngle() / 2f, vacuumSystem.GetSuctionAngle() / 2f);
            }

            yield return null;
        }

        activeLines.Remove(lineObj);
        Destroy(lineObj);
    }

    void UpdateFlowLines()
    {
        foreach (var lineObj in activeLines)
        {
            if (lineObj == null) continue;
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            if (lr == null) continue;

            // Redibuja en base a la nueva posición del jugador
            Vector3 start = lr.GetPosition(0);
            Vector3 end = lr.GetPosition(1);
            // Se mantiene fluido mientras el jugador se mueve
        }
    }

    void ClearFlowLines()
    {
        foreach (var line in activeLines)
        {
            if (line != null)
                Destroy(line);
        }
        activeLines.Clear();
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (T)field.GetValue(obj) : default;
    }
}
