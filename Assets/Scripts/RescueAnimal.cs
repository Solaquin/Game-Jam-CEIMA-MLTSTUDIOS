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
            RescueUIManager.Instance.UpdateTimer(timer); 

            if (timer <= 0)
            {
                timerActive = false;
                RescueUIManager.Instance.HideTimer();
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

        RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
        if (rescueInteraction != null)
        {
            rescueInteraction.SetCurrentAnimal(this);
        }
    }

    public void ReachBase()
    {
        if (!isRescued) return;

        timerActive = false;
        RescueUIManager.Instance.HideTimer();   

        if (timer > 0)
        {
            Debug.Log($"{data.animalName} llegó a salvo. +{data.rewardMoney} monedas!");
            var diver = FindFirstObjectByType<DiverMovement>();
            GiveReward(diver);
            RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
            if (diver != null)
            {
                rescueInteraction.SetCurrentAnimal(null);
            }
        }
        else
        {
            Debug.Log($"{data.animalName} no sobrevivió al rescate :(");
        }

        hasActiveRescue = false;
        Destroy(gameObject);
    }

    private void GiveReward(DiverMovement diver)
    {
        PlayerStats playerStats = diver.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.money += data.rewardMoney;
        }

    }

    public void RescueFailed()
    {
        Debug.Log($"{data.animalName} no logró sobrevivir al rescate (tiempo agotado).");
        hasActiveRescue = false;
        isRescued = false;
        timerActive = false;
        meshRenderer.enabled = true;
        animalCollider.enabled = true;
        timer = data.rescueTimeLimit;
        RescueUIManager.Instance.HideTimer();

        var diver = FindFirstObjectByType<DiverMovement>();
        RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
        if (diver != null)
        {
            rescueInteraction.SetCurrentAnimal(null);
        }
        
    }
}
