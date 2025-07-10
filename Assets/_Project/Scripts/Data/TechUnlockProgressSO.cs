using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TechUnlockProgess_SO", menuName = "PlayerData/TechUnlockProgess_SO")]

public class TechUnlockProgess_SO : ScriptableObject
{
    public List<int> unlockedItemIDs;     // 已解锁的道具
    public List<int> unlockedMonsterIDs;  // 已解锁的怪物
    public List<int> unlockedSkillIDs;    // 已解锁的技能

    public TechUnlockSaveData GetSaveData()
    {
        return new TechUnlockSaveData
        {
            unlockedItemIDs = new List<int>(unlockedItemIDs),
            unlockedMonsterIDs = new List<int>(unlockedMonsterIDs),
            unlockedSkillIDs = new List<int>(unlockedSkillIDs)
        };
    }

    // 从存档数据还原
    public void LoadFromSaveData(TechUnlockSaveData data)
    {
        unlockedItemIDs = new List<int>(data.unlockedItemIDs);
        unlockedMonsterIDs = new List<int>(data.unlockedMonsterIDs);
        unlockedSkillIDs = new List<int>(data.unlockedSkillIDs);
    }
}
