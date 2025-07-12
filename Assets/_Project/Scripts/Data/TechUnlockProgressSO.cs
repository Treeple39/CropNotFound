using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TechUnlockProgess_SO", menuName = "PlayerData/TechUnlockProgess_SO")]

public class TechUnlockProgess_SO : ScriptableObject
{
    public List<int> unlockedItemIDs;     // �ѽ����ĵ���
    public List<int> unlockedMonsterIDs;  // �ѽ����Ĺ���
    public List<int> unlockedSkillIDs;    // �ѽ����ļ���

    public TechUnlockSaveData GetSaveData()
    {
        return new TechUnlockSaveData
        {
            unlockedItemIDs = new List<int>(unlockedItemIDs),
            unlockedMonsterIDs = new List<int>(unlockedMonsterIDs),
            unlockedSkillIDs = new List<int>(unlockedSkillIDs)
        };
    }

    // �Ӵ浵���ݻ�ԭ
    public void LoadFromSaveData(TechUnlockSaveData data)
    {
        unlockedItemIDs = new List<int>(data.unlockedItemIDs);
        unlockedMonsterIDs = new List<int>(data.unlockedMonsterIDs);
        unlockedSkillIDs = new List<int>(data.unlockedSkillIDs);
    }
}
