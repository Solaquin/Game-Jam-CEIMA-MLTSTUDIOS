using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BagSystem playerBag;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private DiverMovement diverMovement;
    [SerializeField] private OxygenSystem oxygenSystem;

    [Header("Available Upgrades")]
    [SerializeField] private UpgradeData[] availableUpgrades;

    [Header("Audio")]
    [SerializeField] private AudioClip buySound;  
    [SerializeField] private AudioClip sellSound;   
    [SerializeField] private AudioClip errorSound;  

    // Diccionario temporal con los niveles de mejora actuales
    private Dictionary<UpgradeData, int> upgradeLevels = new();

    public IReadOnlyList<UpgradeData> Upgrades => availableUpgrades;
    private void Awake()
    {
        if (vacuumSystem == null)
            vacuumSystem = FindFirstObjectByType<VacuumSystem>();

        InitializeUpgrades();
    }
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
        //Debug.Log($"VacuumSystem referencia: {vacuumSystem.name}");
        if (!upgradeLevels.ContainsKey(upgrade))
            upgradeLevels[upgrade] = 0;

        int currentLevel = upgradeLevels[upgrade];

        if (currentLevel >= upgrade.maxLevel)
        {
            Debug.Log("Ya alcanzaste el nivel máximo.");
            PlaySound(errorSound);
            return false;
        }

        int cost = upgrade.GetCostAtLevel(currentLevel + 1);

        if (playerStats.money < cost)
        {
            Debug.Log("No tienes suficiente dinero.");
            PlaySound(errorSound);
            return false;
        }
        playerStats.money -= cost;
        upgradeLevels[upgrade]++;
        float newValue = upgrade.GetValueAtLevel(upgradeLevels[upgrade]);

        if (vacuumSystem != null)
        {
            switch (upgrade.ID)
            {
                case 0: //Bag Capacity

                    playerBag.SetMaxBagCapacity(newValue);
                    break;
                case 1: //Flippers
                    diverMovement.SetSwimSpeed(newValue);
                    break;
                case 2: //Oxygen Tank
                    //Debug.Log($"Nuevo valor para Oxígeno: {newValue}");
                    oxygenSystem.SetMaxOxygen(newValue);
                    break;
                case 3: //Vacuum Motor
                    vacuumSystem.SetSuctionRadius(newValue);
                    break;
                case 4: //Vacuum Mouth
                    vacuumSystem.SetSuctionAngle(newValue);
                    break;
            }
        }

        PlaySound(buySound);

        Debug.Log($"Comprado {upgrade.upgradeName}, nuevo nivel: {upgradeLevels[upgrade]}, valor: {newValue}");
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

        PlaySound(sellSound);
        Debug.Log($"Vendido {quantity}x {item.data.name} por {total} monedas.");
        return true;
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
            //Debug.Log($"Sonido reproducido: {clip.name}");
        }
    }
}
