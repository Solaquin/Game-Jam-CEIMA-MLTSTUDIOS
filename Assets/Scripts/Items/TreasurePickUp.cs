using UnityEngine;

public class TreasurePickup : MonoBehaviour
{
    public TreasureData data; // Referencia al ScriptableObject


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.name);
        if (other.CompareTag("Player"))
        {
            BagSystem bag = other.GetComponent<BagSystem>();
            Debug.Log("Player collided with treasure pickup.");
            if (bag != null)
            {
                bool added = bag.AddItem(data, 1);
                if (added)
                    Destroy(gameObject); // Elimina el objeto del mundo
            }
        }
    }
}
