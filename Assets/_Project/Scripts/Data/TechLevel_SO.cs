using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TechLevel_SO", menuName = "PlayerData/TechLevel_SO")]

public class TechLevel_SO : ScriptableObject
{
    public int CurrentLevel;
    public float CurrentPoints;
    public List<TechLevelData> techLevelData;

    public bool EventHasTrigger(int level)
    {
        bool has = techLevelData.FirstOrDefault(i => i.techLevel == level).techLevelEventHasTrigger;
        return has;
    }
}
