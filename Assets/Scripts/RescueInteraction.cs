using UnityEngine;

public class RescueInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask animalLayer;

    private RescueAnimal currentAnimal;

    void Update()
    {
        CheckForAnimal();

            if (currentAnimal != null && Input.GetKeyDown(KeyCode.E))
            {
                currentAnimal.Rescue();
            }
        }

        void CheckForAnimal()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, animalLayer);
            currentAnimal = hits.Length > 0 ? hits[0].GetComponent<RescueAnimal>() : null;
        }
    }

