using UnityEngine;

[CreateAssetMenu(fileName = "TreasureData", menuName = "Items/Treasure")]
public class TreasureData : ScriptableObject
{
    [Header("General Data")]
    public string itemName;
    public Sprite sprite;

    [TextArea]
    public string description;

    [Header("Treasure Specific Data")]
    public int price;
    public float weight;
    public float rarity;
}
