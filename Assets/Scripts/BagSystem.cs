using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public float bagCapacity = 100f; // Maximum weight capacity of the bag

    public List<BagItem> items = new List<BagItem>();

    public bool AddItem(ScriptableObject item, int amount = 1)
    {
        float currentWeight = GetCurrentWeight();
        float itemWeight = 0f;

        if (item is TreasureData treasure)
        {
            itemWeight = treasure.weight * amount;
        }
        else if (item is TrashData trash)
        {
            itemWeight = trash.weight * amount;
        }
        else
        {
            Debug.Log("Item type not recognized.");
            return false;
        }

        if (currentWeight + itemWeight > bagCapacity)
        {
            Debug.Log("Cannot add item. Bag capacity exceeded.");
            return false;
        }

        //Verificar si el item ya existe en la bolsa
        BagItem existingItem = items.Find(x => x.data == item);
        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            BagItem newItem = new BagItem
            {
                data = item,
                quantity = amount
            };
            items.Add(newItem);
        }
        return true;
    }

    public void RemoveItem(ScriptableObject item, int amount = 1)
    {
        BagItem existingItem = items.Find(x => x.data == item);
        if (existingItem != null)
        {
            existingItem.quantity -= amount;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
        }
    }

    float GetCurrentWeight()
    {
        float totalWeight = 0f;
        foreach (BagItem bagItem in items)
        {
            totalWeight += bagItem.Weight;
        }
        return totalWeight;
    }
}
