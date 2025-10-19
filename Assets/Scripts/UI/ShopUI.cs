using System.Collections.Generic;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;



public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopSystem shopSystem;
    [SerializeField] private BagSystem playerBag;

    [Header("Buy Elements")]
    [SerializeField] private GameObject buySlotParent;
    [SerializeField] private GameObject buyContentPanel;
    [SerializeField] private GameObject buySlotPrefab;

    [Header("Sell Elements")]
    [SerializeField] private GameObject sellSlotParent;
    [SerializeField] private GameObject sellContentPanel;
    [SerializeField] private GameObject sellSlotPrefab;

    private readonly List<ShopISlotUI> activeBuySlots = new();
    private readonly List<ShopISlotUI> activeSellSlots = new();

    private enum ShopMode { Buy, Sell }
    private ShopMode currentMode = ShopMode.Buy;

    private void Start()
    {
        shopSystem.InitializeUpgrades();
    }
    private void OnEnable()
    {
        ShowSellFrame();
        ShowBuyFrame();
        RefreshShopUI();
    }

    public void ShowBuyFrame()
    {
        SetShopMode(ShopMode.Buy);
    }

    public void ShowSellFrame()
    {
        SetShopMode(ShopMode.Sell);
    }

    private void SetShopMode(ShopMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;

        bool isSell = (mode == ShopMode.Sell);

        sellSlotParent.SetActive(isSell);
        buySlotParent.SetActive(!isSell);
        RefreshShopUI();
    }
    public void RefreshShopUI()
    {
        ClearSlots();

        if (currentMode == ShopMode.Buy)
        {

            foreach (var upgrade in shopSystem.Upgrades)
            {
                var slotObj = Instantiate(buySlotPrefab, buyContentPanel.transform);
                var slot = slotObj.GetComponent<ShopISlotUI>();

                slot.SetBuyItem(upgrade, shopSystem.GetUpgradeLevel(upgrade), OnBuyUpgrade);
                activeBuySlots.Add(slot);
            }
        }
        else // Sell mode
        {
            foreach (var item in playerBag.items)
            {

                var slotObj = Instantiate(sellSlotPrefab, sellContentPanel.transform);
                var slot = slotObj.GetComponent<ShopISlotUI>();

                slot.SetSellItem(item, OnSellItem);
                activeSellSlots.Add(slot);
            }
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in activeBuySlots)
            Destroy(slot.gameObject);
        foreach (var slot in activeSellSlots)
            Destroy(slot.gameObject);

        activeBuySlots.Clear();
        activeSellSlots.Clear();
    }

    private void OnBuyUpgrade(UpgradeData upgrade)
    {
        if (shopSystem.TryBuyUpgrade(upgrade))
            RefreshShopUI();
    }

    private void OnSellItem(BagItem item)
    {
        int quantity = 1;

        if (shopSystem.TrySellItem(item, quantity))
            RefreshShopUI();
    }

    private int GetItemPrice(BagItem item)
    {
        return item.data switch
        {
            TreasureData treasure => treasure.price,
            TrashData trash => trash.price,
            _ => 0
        };
    }


}
