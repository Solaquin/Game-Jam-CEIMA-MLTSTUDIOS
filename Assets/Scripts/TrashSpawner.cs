using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Basura")]
    public List<GameObject> trashPrefabs = new List<GameObject>();

    [Header("Spawn Area")]
    public BoxCollider spawnArea;
    public float minSeparation = 0.5f;
    public int maxAttemptsPerTrash = 20;

    [Header("Cantidad")]
    public bool randomCount = true;
    public int fixedCount = 10;
    public Vector2 randomCountRange = new Vector2(5, 15);

    [Header("Z Layer")]
    public float zLayer = 0f;

    [Header("Colisiones")]
    public LayerMask spawnLayerMask = ~0;

    [Header("Flotación (opcional)")]
    public bool enableFloat = true;
    public float floatSpeed = 0.5f;
    public float floatAmplitude = 0.3f;

    private static readonly List<Collider> trashColliders = new List<Collider>();

    void Start()
    {
        if (trashPrefabs == null || trashPrefabs.Count == 0 || spawnArea == null)
        {
            Debug.LogWarning("TrashSpawner: faltan prefabs o área de spawn.");
            return;
        }

        spawnArea.isTrigger = true;
        Bounds bounds = spawnArea.bounds;

        int count = randomCount
            ? Random.Range((int)randomCountRange.x, (int)randomCountRange.y + 1)
            : Mathf.Max(1, fixedCount);

        for (int i = 0; i < count; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerTrash && !placed; attempt++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    zLayer
                );

                // Evita superposición
                if (Physics.OverlapSphere(pos, minSeparation, spawnLayerMask, QueryTriggerInteraction.Ignore).Length > 0)
                    continue;

                GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
                GameObject trash = Instantiate(prefab, pos, Quaternion.identity);
                trash.layer = LayerMask.NameToLayer("collectibleLayer");
                // Añadir flotación si está activada
                //if (enableFloat)
                //{
                //    TrashFloat floatScript = trash.AddComponent<TrashFloat>();
                //    floatScript.floatSpeed = floatSpeed;
                //    floatScript.floatAmplitude = floatAmplitude;
                //}

                var col = trash.GetComponent<Collider>();
                if (col != null)
                {
                    foreach (var other in trashColliders)
                        if (other) Physics.IgnoreCollision(col, other, true);

                    trashColliders.Add(col);
                }

                placed = true;
            }
        }
    }
}

public class TrashFloat : MonoBehaviour
{
    public float floatSpeed = 0.3f;
    public float floatAmplitude = 0.2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }
}
