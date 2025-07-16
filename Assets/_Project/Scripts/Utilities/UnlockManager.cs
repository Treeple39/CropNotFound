using System.Collections.Generic;
using System;
using UnityEngine;
using Inventory;
using UnityEngine.SceneManagement;

public class UnlockManager : Singleton<UnlockManager>
{
    [SerializeField] private TechUnlockProgess_SO techUnlockSO;
    private Dictionary<TechLevelUnlockEventType, Func<int, Details>> _unlockMethods;

    [Header("һ���Խ����¼�,����������")]
    [SerializeField]
    public List<UnlockHintEntry> unlockHintList;
    public Dictionary<int, SingleUnlockHintsData> triggeredUnlockHints { get; private set; }


    public void Init()
    {
        _unlockMethods = new Dictionary<TechLevelUnlockEventType, Func<int, Details>>()
        {
            { TechLevelUnlockEventType.UnlockItem, DataManager.Instance.GetItemDetail },
            { TechLevelUnlockEventType.UnlockMonster, DataManager.Instance.GetMonsterDetail },
            { TechLevelUnlockEventType.UnlockSkill, DataManager.Instance.GetSkillDetail }
        };

        techUnlockSO = DataManager.Instance.techUnlockProgess;

        triggeredUnlockHints = new Dictionary<int, SingleUnlockHintsData>();
        foreach (var entry in unlockHintList)
        {
            if (!triggeredUnlockHints.ContainsKey(entry.key))
            {
                triggeredUnlockHints.Add(entry.key, entry.value);
            }
        }

    }

    public Details GetUnlockDetail(TechLevelUnlockEventType unlockType, int rawID)
    {
        if (_unlockMethods.TryGetValue(unlockType, out var method))
        {
            return method(rawID);
        }
        throw new ArgumentException($"Unknown unlock type: {unlockType}");
    }

    //��UI�õ�
    public LevelUpContentData GetContentData(TechLevelUnlockEventType unlockType, int id)
    {
        if (unlockType == TechLevelUnlockEventType.PlayStory || unlockType == TechLevelUnlockEventType.Default)
            return null;

        Details detail = GetUnlockDetail(unlockType, id);
        if (detail == null) return null;

        string typeTip = unlockType switch
        {
            TechLevelUnlockEventType.UnlockItem => "����",
            TechLevelUnlockEventType.UnlockMonster => "����",
            TechLevelUnlockEventType.UnlockSkill => "����",
            _ => "δ֪"
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
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.OnTechLevelUpEvent -= UnlockItem;
        EventHandler.OnTechLevelUpEvent -= UnlockEnemy;
        EventHandler.OnTechLevelUpEvent -= UnlockSkill;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            TryTriggerUnlockEventOnce(1002, () =>
            {
                InventoryManager.Instance.AddItem(1002);
                UIManager.Instance.ShowHighlight(UIManager.Instance.BagPanel);
            });

            TryTriggerUnlockEventOnce(1000, () =>
            {
                InventoryManager.Instance.AddItem(1000);
            });

            TryTriggerUnlockEventOnce(6000);
        }
    }

    private void TryTriggerUnlockEventOnce(int unlockID, Action onTrigger = null)
    {
        if (triggeredUnlockHints.TryGetValue(unlockID, out SingleUnlockHintsData data) && data.triggered)
            return;

        Action action = data.unlockType switch
        {
            TechLevelUnlockEventType.UnlockItem => () => {
                if (techUnlockSO.unlockedItemIDs.Contains(unlockID))
                {
                    data.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText);
                    onTrigger?.Invoke();
                }
            }
            ,
            TechLevelUnlockEventType.UnlockMonster => () => {
                if (techUnlockSO.unlockedMonsterIDs.Contains(unlockID))
                {
                    data.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText);
                    onTrigger?.Invoke();
                }
            }
            ,
            TechLevelUnlockEventType.UnlockSkill => () => {
                if (techUnlockSO.unlockedSkillIDs.Contains(unlockID))
                {
                    data.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText);
                    onTrigger?.Invoke();
                }
            }
            ,
            _ => null,
        };

        action.Invoke();
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
        Debug.Log($"��⵽�¼��ܽ����¼���ID: {skillID}");
        techUnlockSO.unlockedSkillIDs.Add(skillID);
        BuffApplicationManager.Instance.ApplySingleBuff(skillID);
        DataManager.Instance.SaveDynamicData(techUnlockSO, "TechUnlockProgess.json");
    }
}
