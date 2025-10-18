using UnityEngine;

[System.Serializable]
public class BagItem
{
    public ScriptableObject data;
    public int quantity;
    public float Weight => GetWeight();

    float GetWeight()
    {
        switch (data)
        {
            case TreasureData treasure: return treasure.weight * quantity;
            case TrashData trash: return trash.weight * quantity;
            default: return 0f;
        }
    }
}
