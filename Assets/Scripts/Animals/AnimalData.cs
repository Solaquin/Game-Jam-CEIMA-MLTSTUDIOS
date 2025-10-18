using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "Animals/AnimalData")]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public int rewardMoney;
    public float rescueTimeLimit;
}
