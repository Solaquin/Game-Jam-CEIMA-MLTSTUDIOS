using UnityEngine;

public class RescueAnimal : MonoBehaviour
{
    [Header("Animal Data")]
    public AnimalData data;

    private bool isRescued = false;
    private bool timerActive = false;
    private float timer;

    public static bool hasActiveRescue = false; // Evita más de un rescate a la vez

    private Renderer meshRenderer;
    private Collider animalCollider;

    private void Start()
    {
        if (data != null)
            timer = data.rescueTimeLimit;

        meshRenderer = GetComponentInChildren<Renderer>();
        animalCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;
            Debug.Log($"[TIMER] {data.animalName}: {timer:F1}s");

            if (timer <= 0)
            {
                timerActive = false;
                RescueFailed();
            }
        }
    }

    public void Rescue(DiverMovement diver)
    {
        if (isRescued || hasActiveRescue)
        {
            Debug.Log("Ya estás rescatando a un animal o este ya fue rescatado.");
            return;
        }

        isRescued = true;
        timerActive = true;
        hasActiveRescue = true;

        Debug.Log($"{data.animalName} rescatado. Tiempo: {timer:F1}s para volver a base.");

        // En lugar de SetActive(false), solo ocultamos el modelo
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (animalCollider != null) animalCollider.enabled = false;

        diver.SetRescuedAnimal(this);

    }

    public void ReachBase()
    {
        if (!isRescued) return;

        timerActive = false;

        if (timer > 0)
        {
            Debug.Log($"{data.animalName} llegó a salvo. +{data.rewardMoney} monedas!");
            GiveReward();
        }
        else
        {
            Debug.Log($"{data.animalName} no sobrevivió al rescate :(");
        }

        hasActiveRescue = false;
        Destroy(gameObject);
    }

    private void GiveReward()
    {
        // Ejemplo de recompensa
        // PlayerStats.Instance.AddMoney(data.rewardMoney);
    }

    private void RescueFailed()
    {
        Debug.Log($"{data.animalName} no logró sobrevivir al rescate (tiempo agotado).");
        hasActiveRescue = false;
        isRescued = false;
        // Opcional: destruir el objeto si querés que desaparezca tras fallar
        Destroy(gameObject);
    }
}
