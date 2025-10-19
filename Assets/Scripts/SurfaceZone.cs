using UnityEngine;
using TMPro;

public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressEText;

    private Vector3 baseSpawnPoint = new Vector3(-18f, 21f, 0f);
    private Vector3 waterSpawnPoint = new Vector3(-6f, 14f, 0f);

    private bool isPlayerNearby = false;
    private bool isAtBase = false; 
    private DiverMovement diver;

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
            diver = other.GetComponent<DiverMovement>();
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

    private void GoToBase()
    {
        if (diver != null)
        {
            diver.TeleportToBase(baseSpawnPoint);

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
            diver.TeleportToWater(waterSpawnPoint);
            OxygenSystem oxygen = diver.GetComponent<OxygenSystem>();
            if (oxygen != null)
                oxygen.SetSafeZone(false);
            if (pressEText != null)
                pressEText.text = "Press E";
            
            isAtBase = false;
        }
    }
}
