using UnityEngine;

public class RescueInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private DiverMovement playerDiverMovementReference; 

    private RescueAnimal closerAnimal;
    private RescueAnimal currentAnimal;

    void Update()
    {
        CheckForAnimal();

        if (closerAnimal != null && !RescueAnimal.hasActiveRescue)
        {
            float timeLimit = closerAnimal.data != null ? closerAnimal.data.rescueTimeLimit : 0f;

            RescueUIManager.Instance.UpdateRescuePrompt(closerAnimal.transform.position, timeLimit);

            if (Input.GetKeyDown(KeyCode.E))
            {
                closerAnimal.Rescue(playerDiverMovementReference);
                
                RescueUIManager.Instance.HideRescuePrompt();
            }
        }
        else
        {
            RescueUIManager.Instance.HideRescuePrompt();
        }
    }

    void CheckForAnimal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, animalLayer);

        closerAnimal = hits.Length > 0
            ? hits[0].GetComponent<RescueAnimal>()
            : null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    public RescueAnimal GetCurrentAnimal()
    {
        return currentAnimal;
    }

    public void SetCurrentAnimal(RescueAnimal animal)
    {
        currentAnimal = animal;
    }

}
