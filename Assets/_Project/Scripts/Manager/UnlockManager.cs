using System.Collections.Generic;
using System;
using UnityEngine;
using Inventory;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UnlockManager : Singleton<UnlockManager>
{
    private TechUnlockProgess_SO techUnlockSO;
    private Dictionary<TechLevelUnlockEventType, Func<int, Details>> _unlockMethods;
    private UnlockHint_SO _runtimeUnlockHints;

    public List<TextColor> textColor;

    public void Init()
    {
        _unlockMethods = new Dictionary<TechLevelUnlockEventType, Func<int, Details>>()
        {
            { TechLevelUnlockEventType.UnlockItem, DataManager.Instance.GetItemDetail },
            { TechLevelUnlockEventType.UnlockMonster, DataManager.Instance.GetMonsterDetail },
            { TechLevelUnlockEventType.UnlockSkill, DataManager.Instance.GetSkillDetail }
        };

        techUnlockSO = DataManager.Instance.techUnlockProgess;
        _runtimeUnlockHints = DataManager.Instance.hintUnlockProgess; 
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
            TechLevelUnlockEventType.UnlockItem => textColor[0].text,
            TechLevelUnlockEventType.UnlockMonster => textColor[1].text,
            TechLevelUnlockEventType.UnlockSkill => textColor[2].text,
            _ => "δ֪"
        };

        Color typeColor = unlockType switch
        {
            TechLevelUnlockEventType.UnlockItem => textColor[0].color,
            TechLevelUnlockEventType.UnlockMonster => textColor[1].color,
            TechLevelUnlockEventType.UnlockSkill => textColor[2].color,
            _ => Color.gray
        };

        return new LevelUpContentData
        {
            contentTitle = detail.Name,
            contentText = detail.Description,
            contentImage = ResourceManager.LoadSprite(detail.IconPath),
            contentTypeTip = typeTip,
            contentTypeColor = typeColor,
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
            TryTriggerUnlockEventOnce();
        }
    }

    //Fill in the ID to trigger the unlock event for the specified ID.
    //If no ID is provided, it will trigger the unlock event for all words at that level.
    private void TryTriggerUnlockEventOnce(int ID = 0, Action onTrigger = null)
    {
        SingleUnlockHintsData data;
        
        if(ID == 0)
        {
            TechLevelEventData techEvent;
            if (DataManager.Instance.TechLevelEventDatas.TryGetValue(TechLevelManager.Instance.CurrentTechLevel - 1, out techEvent))
            {
                if(techEvent.unlockHintID != null && techEvent.unlockHintID.Count != 0 && techEvent.unlockHintID[0] != 0)
                for (int i = 0; i < techEvent.unlockHintID.Count; i++)
                {
                    data = null;
                    if (DataManager.Instance.UnlockHintsData.TryGetValue(techEvent.unlockHintID[i] - 14000, out data))
                    {
                        TriggerUnlockEvent(techEvent.unlockHintID[i], data, onTrigger);
                    }
                }
            }
        }
        if (DataManager.Instance.UnlockHintsData.TryGetValue(ID - 14000, out data)) 
        {
            TriggerUnlockEvent(ID, data, onTrigger);
        }
    }

    private void TriggerUnlockEvent(int ID, SingleUnlockHintsData data, Action onTrigger)
    {
        int unlockID = data.unlockID;
        Action action = data.unlockType switch
        {
            TechLevelUnlockEventType.UnlockItem => () => {
                if (techUnlockSO.unlockedItemIDs.Contains(unlockID))
                {
                    UnlockHintData hint = _runtimeUnlockHints.unlockHintData.Find(i => i.ID == ID);
                    if(hint == null || hint.triggered)
                    {
                        return;
                    }
                    hint.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText, data.messageDuration);
                    onTrigger = new Action(() =>
                    {
                        InventoryManager.Instance.AddItem(unlockID);
                        if(unlockID == 1002)
                            UIManager.Instance.ShowHighlight(UIManager.Instance.BagPanel);
                    });
                    onTrigger.Invoke();
                }
            }
            ,
            TechLevelUnlockEventType.UnlockMonster => () => {
                if (techUnlockSO.unlockedMonsterIDs.Contains(unlockID))
                {
                    UnlockHintData hint = _runtimeUnlockHints.unlockHintData.Find(i => i.ID == ID);
                    if (hint == null || hint.triggered)
                    {
                        return;
                    }
                    hint.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText, data.messageDuration);
                    onTrigger?.Invoke();
                }
            }
            ,
            TechLevelUnlockEventType.UnlockSkill => () => {
                if (techUnlockSO.unlockedSkillIDs.Contains(unlockID))
                {
                    UnlockHintData hint = _runtimeUnlockHints.unlockHintData.Find(i => i.ID == ID);
                    if (hint == null || hint.triggered)
                    {
                        return;
                    }
                    hint.SetBool(true);
                    EventHandler.CallSystemMessageShow(data.messageText, data.messageDuration);
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
