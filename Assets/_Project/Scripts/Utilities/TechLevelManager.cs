using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class TechLevelManager : Singleton<TechLevelManager>
{
    //????????
    public int CurrentTechLevel;
    public float CurrentPoints;
    private TechLevel_SO _runtimeTechLevel;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    //????????????
    private List<(TechLevelUnlockEventType evtType, int id)> _pendingUnlockEvents = new();

    public void demoCall()
    {
        DataManager.Instance.SaveDynamicData(_runtimeTechLevel, "ArchiveTechLevel.json");
        EventHandler.CallTechPointChange(5);
    }

    private void OnEnable()
    {
        EventHandler.OnTechPointsChanged += AddTechPoints;
    }

    private void OnDisable()
    {
        EventHandler.OnTechPointsChanged -= AddTechPoints;
    }

    public void Init()
    {
        _runtimeTechLevel = DataManager.Instance.archiveTechLevel;

        // ????????????
        CurrentTechLevel = _runtimeTechLevel.CurrentLevel;
        CurrentPoints = _runtimeTechLevel.CurrentPoints;

        // ???????????????? = CurrentLevel - 1??
        TechLevelDetails detail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel - 1, out detail))
        {
            UIManager.Instance.TechLevelPanel.pointsLimit = detail.needPoints;
        }

        // ?????UI???
        UIManager.Instance.TechLevelPanel.InitTechLevelUI(CurrentTechLevel, CurrentPoints);
    }

    #region PointsGain
    public void AddTechPoints(float amount)
    {
        CurrentPoints += amount;
        UIManager.Instance.TechLevelPanel.AddPointsUI(CurrentPoints);

        CheckLevelUp();
        _runtimeTechLevel.CurrentPoints = CurrentPoints;


        DataManager.Instance.archiveTechLevel = _runtimeTechLevel;
        DataManager.Instance.SaveDynamicData(_runtimeTechLevel, "ArchiveTechLevel.json");
    }

    private void CheckLevelUp()
    {
        // ??????????????????? = CurrentLevel?????0???1????
        TechLevelDetails nextLevelDetail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out nextLevelDetail))
        {
            if(CurrentPoints >= nextLevelDetail.needPoints)
            {
                CurrentPoints -= nextLevelDetail.needPoints;
                LevelUp();
                
                
                // ??????????????????CurrentLevel??
                TechLevelDetails newLevelDetail;
                if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out newLevelDetail))
                {
                    UIManager.Instance.TechLevelPanel.pointsLimit = newLevelDetail.needPoints;
                }
                // ????UI??????????¡¤????
                EventHandler.CallSystemMessageShow("??§»???????????????????????????");
                UIManager.Instance.TechLevelPanel.LevelUpUI(CurrentTechLevel, CurrentPoints);
                UIManager.Instance.UILevelUpPanel.InitLevel(CurrentTechLevel - 1, CurrentTechLevel);

                // ???????????????
                CheckLevelUp(); // ?????
            }
        }
    }
    #endregion

    private void LevelUp()
    {
        CurrentTechLevel += 1;
        _runtimeTechLevel.CurrentLevel = CurrentTechLevel;

        if (_runtimeTechLevel.EventHasTrigger(CurrentTechLevel))
            return;

        TechLevelEventData data;
        if (DataManager.Instance.TechLevelEventDatas.TryGetValue(CurrentTechLevel - 1, out data))
        {
            _pendingUnlockEvents.Clear();

            for (int i = 0; i < data.triggerEvents.Count; i++)
            {
                if (data.triggerID.Count > i && data.triggerID[i] != 0)
                {

                    //?????????????????????????
                    _pendingUnlockEvents.Add((data.triggerEvents[i], data.triggerID[i]));
                }
                else
                {
                    Debug.LogWarning($"???{i}¦Ä???????triggerID, ????????ID");
                }
            }

            UIManager.Instance.UILevelUpPanel.InitContent(data.triggerEvents.Count, data.triggerEvents, data.triggerID);

            _runtimeTechLevel.techLevelData[CurrentTechLevel - 1].SetBool(true);
        }
    }
    public void TriggerPendingUnlockEvents()
    {
        foreach (var evt in _pendingUnlockEvents)
        {
            DispatchUpgradeEvent(evt.evtType, evt.id);
        }

        _pendingUnlockEvents.Clear();
    }
    private void DispatchUpgradeEvent(TechLevelUnlockEventType evt, int num)
    {
        EventHandler.CallTechLevelUpEvent(CurrentTechLevel, evt, num);
    }
}