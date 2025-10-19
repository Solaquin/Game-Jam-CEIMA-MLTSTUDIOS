using UnityEngine;
using TMPro;

public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressEText;

    public Transform baseSpawnPoint;
    public Transform waterSpawnPoint;
    public DiverMovement diver;

    private bool isPlayerNearby = false;
    private bool isAtBase = false; 
    

    void Start()
    {
        if (pressEText != null)
            pressEText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (isAtBase)
                ReturnToWater();
            else
                GoToBase();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pressEText != null)
            {
                pressEText.gameObject.SetActive(true);
            }
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pressEText != null)
                pressEText.gameObject.SetActive(false);
            isPlayerNearby = false;
        }
    }

    public void GoToBase()
    {
        if (diver != null)
        {
            diver.TeleportToBase(baseSpawnPoint.position);

            OxygenSystem oxygen = diver.GetComponent<OxygenSystem>();
            if (oxygen != null)
                oxygen.RefillOxygen();
                oxygen.SetSafeZone(true);

            if (pressEText != null)
                pressEText.text = "Press E";

            isAtBase = true;
        }
    }

    private void ReturnToWater()
    {
        if (diver != null)
        {
            diver.TeleportToWater(waterSpawnPoint.position);
            OxygenSystem oxygen = diver.GetComponent<OxygenSystem>();
            if (oxygen != null)
                oxygen.SetSafeZone(false);
            if (pressEText != null)
                pressEText.text = "Press E";
            
            isAtBase = false;
        }
    }
}
