using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Prefabs por especie (cada uno con su Animator listo)")]
    [SerializeField] private List<FishMovement> fishPrefabs; // ← usa estos
    [Tooltip("Opcional: si la lista está vacía, usa este prefab genérico")]
    public FishMovement fishPrefab; // fallback

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

    // para ignorar colisiones entre peces
    private static readonly List<Collider> fishColliders = new List<Collider>();
    private Collider[] playerColliders;

    void Start()
    {
        // Validaciones
        if ((fishPrefabs == null || fishPrefabs.Count == 0) && !fishPrefab)
        {
            Debug.LogWarning("FishSpawner: no hay prefabs asignados.");
            return;
        }
        if (!spawnArea) { Debug.LogWarning("FishSpawner: asigna un BoxCollider como spawnArea."); return; }

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

                // evita solapes fuertes al nacer
                if (Physics.OverlapSphere(pos, minSeparation, spawnLayerMask, QueryTriggerInteraction.Ignore).Length > 0)
                    continue;

                // ======= ELECCIÓN DEL PREFAB AL AZAR (OPCIÓN A) =======
                FishMovement prefabToUse = (fishPrefabs != null && fishPrefabs.Count > 0)
                    ? fishPrefabs[Random.Range(0, fishPrefabs.Count)]
                    : fishPrefab;

                var fish = Instantiate(prefabToUse, pos, Quaternion.identity);

                // Config común para cualquier especie
                fish.SetPlayer(player);
                fish.speed = Random.Range(speedRange.x, speedRange.y);
                fish.speedAfter = Random.Range(speedAfterRange.x, speedAfterRange.y);
                fish.SetDepthAndBehavior(zLayer, respondToPlayerAtThisZ);

                // Si tus peces usan límites en Y dentro del área de spawn:
                // (activa limitY en el prefab o llama aquí si tu FishMovement tiene SetYBounds)
                // fish.SetYBounds(b.min.y, b.max.y, true);

                // Ignorar colisiones (player y entre peces)
                var fc = fish.GetComponent<Collider>();
                if (fc != null)
                {
                    if (playerColliders != null)
                        foreach (var pc in playerColliders)
                            if (pc) Physics.IgnoreCollision(fc, pc, true);

                    foreach (var other in fishColliders)
                        if (other) Physics.IgnoreCollision(fc, other, true);

                    fishColliders.Add(fc);
                }

                placed = true;
            }
        }
    }
}
