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
        // Podrías activar una animación o hacer que siga al jugador
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

        Destroy(gameObject);
    }

    private void GiveReward()
    {
        // Aquí sumás dinero al jugador
        //PlayerStats.Instance.AddMoney(data.rewardMoney);
    }

    private void RescueFailed()
    {
        Debug.Log($"{data.animalName} no logró sobrevivir.");
        // Podés penalizar puntos o simplemente mostrar mensaje
    }
}
