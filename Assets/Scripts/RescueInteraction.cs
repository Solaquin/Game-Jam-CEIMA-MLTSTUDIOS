using UnityEngine;

public class RescueInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private DiverMovement playerDiverMovementReference; 

    private RescueAnimal currentAnimal;

    void Update()
    {
        CheckForAnimal();

        if (currentAnimal != null && !RescueAnimal.hasActiveRescue)
        {
            // Mostrar mensaje de interacción
            RescueUIManager.Instance.ShowInteractText(true);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentAnimal.transform.position + Vector3.up * 1.5f);
            RescueUIManager.Instance.interactText.transform.position = screenPos;
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentAnimal.Rescue(playerDiverMovementReference);
                RescueUIManager.Instance.ShowInteractText(false); // ocultar mensaje al rescatar
            }
        }
        else
        {
            RescueUIManager.Instance.ShowInteractText(false);
        }
    }

    void CheckForAnimal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, animalLayer);

        currentAnimal = hits.Length > 0
            ? hits[0].GetComponent<RescueAnimal>()
            : null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
