using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TechLevel_SO", menuName = "PlayerData/TechLevel_SO")]

public class TechLevel_SO : ScriptableObject, IInitializableSO
{
    public int CurrentLevel = 1;
    public float CurrentPoints = 0;
    public List<TechLevelData> techLevelData;

    public bool EventHasTrigger(int level)
    {
        bool has = techLevelData.FirstOrDefault(i => i.techLevel == level).techLevelEventHasTrigger;
        return has;
    }

    public void InitDefault()
    {
        CurrentLevel = 1;
        CurrentPoints = 0;
        techLevelData.Clear();
        for (int i = 1; i <= 100; i++) // 初始化1-100级，每级默认未解锁
        {
            techLevelData.Add(new TechLevelData { techLevel = i, techLevelEventHasTrigger = false });
        }
    }
}
