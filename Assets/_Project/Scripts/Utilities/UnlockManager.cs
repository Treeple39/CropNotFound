using System.Collections.Generic;
using System;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    [SerializeField] private TechUnlockProgess_SO techUnlockSO;
    private Dictionary<TechLevelUnlockEventType, Func<int, Details>> _unlockMethods;

    public void Init()
    {
        _unlockMethods = new Dictionary<TechLevelUnlockEventType, Func<int, Details>>()
        {
            { TechLevelUnlockEventType.UnlockItem, DataManager.Instance.GetItemDetail },
            { TechLevelUnlockEventType.UnlockMonster, DataManager.Instance.GetMonsterDetail },
            { TechLevelUnlockEventType.UnlockSkill, DataManager.Instance.GetSkillDetail }
        };
    }

    public Details GetUnlockDetail(TechLevelUnlockEventType unlockType, int rawID)
    {
        if (_unlockMethods.TryGetValue(unlockType, out var method))
        {
            return method(rawID);
        }
        throw new ArgumentException($"Unknown unlock type: {unlockType}");
    }

    //给UI用的
    public LevelUpContentData GetContentData(TechLevelUnlockEventType unlockType, int id)
    {
        if (unlockType == TechLevelUnlockEventType.PlayStory || unlockType == TechLevelUnlockEventType.Default)
            return null;

        Details detail = GetUnlockDetail(unlockType, id);
        if (detail == null) return null;

        string typeTip = unlockType switch
        {
            TechLevelUnlockEventType.UnlockItem => "道具",
            TechLevelUnlockEventType.UnlockMonster => "怪物",
            TechLevelUnlockEventType.UnlockSkill => "技艺",
            _ => "未知"
        };

        return new LevelUpContentData
        {
            contentTitle = detail.Name,
            contentText = detail.Description,
            contentImage = ResourceManager.LoadSprite(detail.IconPath),
            contentTypeTip = typeTip
        };
    }

    private void OnEnable()
    {
        EventHandler.OnTechLevelUpEvent += UnlockItem;
        EventHandler.OnTechLevelUpEvent += UnlockEnemy;
        EventHandler.OnTechLevelUpEvent += UnlockSkill;
    }

    private void OnDisable()
    {
        EventHandler.OnTechLevelUpEvent -= UnlockItem;
        EventHandler.OnTechLevelUpEvent -= UnlockEnemy;
        EventHandler.OnTechLevelUpEvent -= UnlockSkill;
    }

    private void UnlockItem(int techLevel, TechLevelUnlockEventType eventType, int num)
    {
        if (eventType == TechLevelUnlockEventType.UnlockItem)
        {
            if (!techUnlockSO.unlockedItemIDs.Contains(num))
                techUnlockSO.unlockedItemIDs.Add(num);
            DataManager.Instance.SaveDynamicData(techUnlockSO, "TechUnlockProgess.json");
        }
    }

    private void UnlockEnemy(int techLevel, TechLevelUnlockEventType eventType, int num)
    {
        if (eventType == TechLevelUnlockEventType.UnlockMonster)
        {
            if (!techUnlockSO.unlockedMonsterIDs.Contains(num))
                techUnlockSO.unlockedMonsterIDs.Add(num);
            DataManager.Instance.SaveDynamicData(techUnlockSO, "TechUnlockProgess.json");
        }
    }

    private void UnlockSkill(int techLevel, TechLevelUnlockEventType eventType, int skillID)
    {
        if (eventType != TechLevelUnlockEventType.UnlockSkill)
        {
            return;
        }
        if (techUnlockSO.unlockedSkillIDs.Contains(skillID))
        {
            return;
        }
        Debug.Log($"检测到新技能解锁事件！ID: {skillID}");
        techUnlockSO.unlockedSkillIDs.Add(skillID);
        BuffApplicationManager.Instance.ApplySingleBuff(skillID);
        DataManager.Instance.SaveDynamicData(techUnlockSO, "TechUnlockProgess.json");
    }
}
