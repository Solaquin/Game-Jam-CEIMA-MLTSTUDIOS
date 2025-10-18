using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] BagSystem bagSystem;
    [SerializeField] Transform slotFrame;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] TextMeshProUGUI weightText;

    private List<ItemSlotUI> activeSlots = new List<ItemSlotUI>();

    private float bagCapacity;
    private float currentBagLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        // Limpiar slots previos
        foreach (Transform child in slotFrame)
            Destroy(child.gameObject);
        activeSlots.Clear();

        // Crear un slot por ítem
        foreach (var bagItem in bagSystem.items)
        {
            var slotObj = Instantiate(slotPrefab, slotFrame);
            var slotUI = slotObj.GetComponent<ItemSlotUI>();
            slotUI.SetItem(bagItem);
            activeSlots.Add(slotUI);
        }

        bagCapacity = bagSystem.bagCapacity;
        currentBagLoad = bagSystem.currentWeight;
        weightText.text = $"{currentBagLoad}/{bagCapacity}";

    }
    void OnEnable() => bagSystem.OnInventoryChanged += RefreshInventory;
    void OnDisable() => bagSystem.OnInventoryChanged -= RefreshInventory;
}