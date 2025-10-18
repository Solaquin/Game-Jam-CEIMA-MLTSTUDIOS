using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Items/UpgradeData")]
public class UpgradeData : ScriptableObject
{

    [Header("General Data")]
    public string upgradeName;
    public Sprite sprite;

    [TextArea]
    public string description;

    [Header("Upgrade Specific Data")]
    public int maxLevel;
    public int[] costPerLevel;

    public float baseValue;
    public float incrementPerLevel;

    public float GetValueAtLevel(int level)
    {
        return baseValue + incrementPerLevel * (level - 1);
    }

    public int GetCostAtLevel(int level)
    {
        return costPerLevel[level - 1];
    }
}
