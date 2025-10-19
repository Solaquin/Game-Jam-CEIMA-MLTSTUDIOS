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

    private readonly List<GameObject> spawned = new List<GameObject>();

    private static readonly List<Collider> trashColliders = new List<Collider>();

    void Start()
    {
        SpawnAll();
    }

    public void RespawnAll()
    {
        Debug.Log("Respane de bazura con exito");
        ClearSpawned();
        SpawnAll();
    }
    public void ClearSpawned()
    {
        foreach(var go in spawned)
        {
            if (!go) continue;

            var col = go.GetComponent<Collider>();
            if (col)
            {
                foreach (var other in trashColliders)
                    if (other && col) Physics.IgnoreCollision(col, other, false);

                col.enabled = false;
                trashColliders.Remove(col); 
            }
            Destroy(go);
        }
        spawned.Clear();
        trashColliders.RemoveAll(c => c == null);
    }


    private void SpawnAll()
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
            for (int attempt = 0; attempt < maxAttemptsPerTrash; attempt++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    zLayer
                );

                if (Physics.OverlapSphere(pos, minSeparation, spawnLayerMask, QueryTriggerInteraction.Ignore).Length > 0)
                    continue;

                GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
                GameObject trash = Instantiate(prefab, pos, Quaternion.identity);
                spawned.Add(trash);

                int layer = LayerMask.NameToLayer("collectibleLayer");
                if (layer != -1) trash.layer = layer;

                var col = trash.GetComponent<Collider>();
                if (col != null)
                {
                    foreach (var other in trashColliders)
                        if (other) Physics.IgnoreCollision(col, other, true);

                    trashColliders.Add(col);
                }

                break;
            }
        }
    }
}
