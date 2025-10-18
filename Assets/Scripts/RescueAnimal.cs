using UnityEngine;

public class RescueAnimal : MonoBehaviour
{
    [Header("Animal Data")]
    public AnimalData data;

    private bool isRescued = false;
    private bool timerActive = false;
    private float timer;

    void Start()
    {
        if (data != null)
            timer = data.rescueTimeLimit;
    }

    void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timerActive = false;
                RescueFailed();
            }
        }
    }

    public void Rescue()
    {
        if (isRescued) return;

        isRescued = true;
        timerActive = true;
        Debug.Log($"{data.animalName} rescatado. Tiempo: {timer:F1}s para volver a base");
        // Podr�as activar una animaci�n o hacer que siga al jugador
    }

    public void ReachBase()
    {
        if (!isRescued) return;

        timerActive = false;

        if (timer > 0)
        {
            Debug.Log($"{data.animalName} lleg� a salvo. +{data.rewardMoney} monedas!");
            GiveReward();
        }
        else
        {
            Debug.Log($"{data.animalName} no sobrevivi� al rescate :(");
        }

        Destroy(gameObject);
    }

    private void GiveReward()
    {
        // Aqu� sum�s dinero al jugador
        //PlayerStats.Instance.AddMoney(data.rewardMoney);
    }

    private void RescueFailed()
    {
        Debug.Log($"{data.animalName} no logr� sobrevivir.");
        // Pod�s penalizar puntos o simplemente mostrar mensaje
    }
}
