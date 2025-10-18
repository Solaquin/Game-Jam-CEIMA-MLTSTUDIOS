using UnityEngine;

[CreateAssetMenu(fileName = "TrashData", menuName = "Items/Trash")]
public class TrashData : ScriptableObject
{
    [Header("General Data")]
    public string itemName;
    public Sprite sprite;

    [TextArea]
    public string description;

    [Header("Trash Specific Data")]
    public int price;
    public float weight;
    public float rarity;
}
