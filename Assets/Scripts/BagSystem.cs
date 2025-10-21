using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public float bagCapacity = 100f; // Maximum weight capacity of the bag
    public float currentWeight = 0f; // Current weight of items in the bag

    public List<BagItem> items = new List<BagItem>();

    public event System.Action OnInventoryChanged;

    public bool AddItem(ScriptableObject item, int amount = 1)
    {
        currentWeight = GetCurrentWeight();

        if (canAddNextItem(item, amount) == false)
            return false;
        

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

        currentWeight = GetCurrentWeight();

        OnInventoryChanged?.Invoke();
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

            currentWeight = GetCurrentWeight();
            OnInventoryChanged?.Invoke();
        }
    }

    public float GetCurrentWeight()
    {
        float totalWeight = 0f;
        foreach (BagItem bagItem in items)
        {
            totalWeight += bagItem.Weight;
        }
        return totalWeight;
    }

    public void SetMaxBagCapacity(float newCapacity)
    {
        bagCapacity = newCapacity;
        OnInventoryChanged?.Invoke();
    }

    public void ClearBag()
    {
        items.Clear();
        currentWeight = 0f;
        OnInventoryChanged?.Invoke();
    }

    public bool canAddNextItem(ScriptableObject item, int amount)
    {
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

        return true;
    }
}
