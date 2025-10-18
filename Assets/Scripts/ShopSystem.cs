using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BagSystem playerBag;
    [SerializeField] private PlayerStats playerStats;

    [Header("Available Upgrades")]
    [SerializeField] private UpgradeData[] availableUpgrades;

    // Diccionario temporal con los niveles de mejora actuales
    private Dictionary<UpgradeData, int> upgradeLevels = new();

    public IReadOnlyList<UpgradeData> Upgrades => availableUpgrades;

    public void InitializeUpgrades()
    {
        foreach (var upgrade in availableUpgrades)
        {
            if (!upgradeLevels.ContainsKey(upgrade))
                upgradeLevels.Add(upgrade, 0);
        }
    }

    public int GetUpgradeLevel(UpgradeData upgrade)
    {
        return upgradeLevels.ContainsKey(upgrade) ? upgradeLevels[upgrade] : 0;
    }

    public bool TryBuyUpgrade(UpgradeData upgrade)
    {
        if (!upgradeLevels.ContainsKey(upgrade))
            upgradeLevels[upgrade] = 0;

        int currentLevel = upgradeLevels[upgrade];

        if (currentLevel >= upgrade.maxLevel)
        {
            Debug.Log("Ya alcanzaste el nivel máximo.");
            return false;
        }

        int cost = upgrade.GetCostAtLevel(currentLevel + 1);

        if (playerStats.money < cost)
        {
            Debug.Log("No tienes suficiente dinero.");
            return false;
        }

        playerStats.money -= cost;
        upgradeLevels[upgrade]++;

        Debug.Log($"Comprado {upgrade.upgradeName}, nuevo nivel: {upgradeLevels[upgrade]}");
        return true;
    }

    public bool TrySellItem(BagItem item, int quantity)
    {
        int pricePerItem = item.data switch
        {
            TreasureData treasure => treasure.price,
            TrashData trash => trash.price,
            _ => 0
        };

        if (pricePerItem == 0)
            return false;

        int total = pricePerItem * quantity;

        // Quitar del inventario
        playerBag.RemoveItem(item.data, quantity);

        // Sumar dinero
        playerStats.money += total;

        Debug.Log($"Vendido {quantity}x {item.data.name} por {total} monedas.");
        return true;
    }
}
