using UnityEngine;

[CreateAssetMenu(fileName = "RarityDatabase", menuName = "Database/Rarity Database")]
public class RarityDatabase : ScriptableObject
{
    public RarityData[] rarities;

    public RarityData GetRarityData(Rarity rarity)
    {
        foreach (var data in rarities)
        {
            if (data.rarity == rarity)
                return data;
        }
        return rarities[0];
    }
}