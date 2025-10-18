using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopISlotUI : MonoBehaviour
{
    [Header("UI General References")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    [Header("UI Sell Specific References")]
    [SerializeField] private TextMeshProUGUI itemQuantityText;

    [Header("UI Buy Specific References")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Buttons")]
    [SerializeField] private Button actionButton;

    private System.Action<UpgradeData> onBuyCallback;
    private System.Action<BagItem> onSellCallback;

    private UpgradeData currentUpgrade;
    private BagItem currentBagItem;
    private int currentUpgradeLevel;

    public void SetBuyItem(UpgradeData data, int currentLevel, System.Action<UpgradeData> onBuy)
    {
        currentUpgrade = data;
        currentUpgradeLevel = currentLevel;
        onBuyCallback = onBuy;

        itemNameText.text = data.upgradeName;
        itemIconImage.sprite = data.sprite;
        itemDescriptionText.text = data.description;
        levelText.text = $"Nivel: {currentLevel}/{data.maxLevel}";

        int nextCost = (currentLevel < data.maxLevel)
            ? data.GetCostAtLevel(currentLevel + 1)
            : 0;

        itemPriceText.text = currentLevel < data.maxLevel
            ? $"Costo: {nextCost}"
            : "Nivel Máximo";

        actionButton.onClick.RemoveAllListeners();
        actionButton.interactable = currentLevel < data.maxLevel;
        actionButton.onClick.AddListener(() => onBuyCallback?.Invoke(currentUpgrade));
    }

    public void SetSellItem(BagItem bagItem, System.Action<BagItem> onSell)
    {
        currentBagItem = bagItem;
        onSellCallback = onSell;

        Sprite icon = null;
        string name = "";
        string description = "";
        int price = 0;

        switch (bagItem.data)
        {
            case TreasureData treasure:
                icon = treasure.sprite;
                name = treasure.itemName;
                price = treasure.price;
                description = treasure.description;
                break;

            case TrashData trash:
                icon = trash.sprite;
                name = trash.itemName;
                price = trash.price;
                description = trash.description;
                break;
        }

        itemNameText.text = name;
        itemIconImage.sprite = icon;
        itemQuantityText.text = $"x{bagItem.quantity}";
        itemPriceText.text = $"Precio: {price}";
        itemDescriptionText.text = description;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onSellCallback?.Invoke(bagItem));
    }


}
