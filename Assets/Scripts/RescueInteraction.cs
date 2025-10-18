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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAnimal != null)
            {
                currentAnimal.Rescue(playerDiverMovementReference);
            }
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
