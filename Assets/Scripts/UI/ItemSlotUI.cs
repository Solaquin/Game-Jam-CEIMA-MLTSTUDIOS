using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemQuantityText;
    [SerializeField] private Image itemIconImage;

   public void SetBagItem(BagItem item)
   {
        if(item == null || item.data == null)
        {
            itemIconImage.enabled = false;
            itemQuantityText.text = "";
            itemNameText.text = "Invalid Item";
            return;
        }

        Sprite sprite = null;

        switch(item.data)
        {
            case TreasureData treasure:
                itemNameText.text = treasure.itemName;
                itemQuantityText.text = item.quantity.ToString();
                sprite = treasure.sprite;
                break;

            case TrashData trash:
                itemNameText.text = trash.itemName;
                itemQuantityText.text = item.quantity.ToString();
                sprite = trash.sprite;
                break;

            default:
                itemNameText.text = "Unknown Item";
                itemQuantityText.text = "";
                break;
        }

        itemIconImage.sprite = sprite;
        itemIconImage.enabled = sprite != null;
    }
}
