using UnityEngine;

public class TrashPickUp : MonoBehaviour
{
    public TrashData data; // Reference to the ScriptableObject

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.name);
        if (other.CompareTag("Player"))
        {
            BagSystem bag = other.GetComponent<BagSystem>();
            Debug.Log("Player collided with trash pickup.");
            if (bag != null)
            {
                bool added = bag.AddItem(data, 1);
                if (added)
                    Destroy(gameObject); // Remove the object from the world
            }
        }
    }
}
