using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressEText;

    public Transform baseSpawnPoint;
    public Transform waterSpawnPoint;
    public DiverMovement diver;

    private bool isPlayerNearby = false;
    private bool isAtBase = false;
    public List<TrashSpawner> trashSpawner = new List<TrashSpawner>();

    void Start()
    {
        if (pressEText != null)
        {
            pressEText.gameObject.SetActive(false);
        }
            
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (isAtBase)
            {
                ReturnToWater();
            }

            else
            {
                GoToBase();
            }
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
            {
                pressEText.gameObject.SetActive(false);
            }
                
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
            {
                oxygen.RefillOxygen();
                oxygen.SetSafeZone(true);
            }

            RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
            RescueAnimal rescuedAnimal = rescueInteraction.GetCurrentAnimal();

            if (rescuedAnimal != null)
            {
                rescuedAnimal.ReachBase();
            }

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
            {
                oxygen.SetSafeZone(false);
            }
                
            if (pressEText != null)
            {
                pressEText.text = "Press E";
            }

            for (int i = 0; i < trashSpawner.Count; i++)
            {
                var sp = trashSpawner[i];
                if (sp != null) sp.RespawnAll();
            }
            
            isAtBase = false;
        }
    }

    public bool IsAtBase()
    {
        return isAtBase;
    }
}
