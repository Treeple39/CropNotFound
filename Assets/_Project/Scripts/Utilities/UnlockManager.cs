using System.Collections.Generic;
using System;

public class UnlockManager : Singleton<UnlockManager>
{
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
}
