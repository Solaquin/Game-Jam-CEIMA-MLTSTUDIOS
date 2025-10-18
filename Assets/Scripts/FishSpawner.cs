using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    public FishMovement fishPrefab;
    public Transform player;

    public BoxCollider spawnArea;
    public float minSeparation = 0.5f;
    public int maxAttemptsPerFish = 20;

    public float zLayer = 0f;
    public bool respondToPlayerAtThisZ = true;

    public Vector2 speedRange = new Vector2(1.5f, 3.5f);
    public Vector2 speedAfterRange = new Vector2(5f, 8f);

    public bool randomCount = true;
    public int fixedCount = 5;

    public LayerMask spawnLayerMask = ~0;

    // Compartido por todos los spawners para ignorar colisiones entre todos los peces
    private static readonly List<Collider> fishColliders = new List<Collider>();
    private Collider[] playerColliders;

    void Start()
    {
        if (!fishPrefab || !spawnArea) return;

        playerColliders = (player != null) ? player.GetComponentsInChildren<Collider>(true) : null;

        int count = randomCount ? Random.Range(1, 11) : Mathf.Clamp(fixedCount, 1, 10);
        spawnArea.isTrigger = true;
        Bounds b = spawnArea.bounds;

        for (int i = 0; i < count; i++)
        {
            bool placed = false;

            for (int attempt = 0; attempt < maxAttemptsPerFish && !placed; attempt++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(b.min.x, b.max.x),
                    Random.Range(b.min.y, b.max.y),
                    zLayer
                );

                if (Physics.OverlapSphere(pos, minSeparation, spawnLayerMask, QueryTriggerInteraction.Ignore).Length > 0)
                    continue;

                var fish = Instantiate(fishPrefab, pos, Quaternion.identity);
                fish.SetPlayer(player);
                fish.speed = Random.Range(speedRange.x, speedRange.y);
                fish.speedAfter = Random.Range(speedAfterRange.x, speedAfterRange.y);
                fish.SetDepthAndBehavior(zLayer, respondToPlayerAtThisZ);

                var fc = fish.GetComponent<Collider>();
                if (fc != null)
                {
                    // Ignorar player
                    if (playerColliders != null)
                        foreach (var pc in playerColliders)
                            if (pc) Physics.IgnoreCollision(fc, pc, true);

                    // Ignorar peces anteriores (bidireccional con una sola llamada)
                    foreach (var other in fishColliders)
                        if (other) Physics.IgnoreCollision(fc, other, true);

                    fishColliders.Add(fc);
                }

                placed = true;
            }
        }
    }
}
