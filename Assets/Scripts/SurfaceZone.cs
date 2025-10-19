using UnityEngine;
using TMPro;

public class SurfaceZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressEText;

    private Vector3 baseSpawnPoint = new Vector3(0f, 33f, 0f);
    private Vector3 waterSpawnPoint = new Vector3(9f, 30f, 0f);

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
            if (pressEText != null)
                pressEText.text = "Press E";

            isAtBase = false;
        }
    }
}
