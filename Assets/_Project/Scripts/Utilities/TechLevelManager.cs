using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class TechLevelManager : Singleton<TechLevelManager>
{
    public int CurrentTechLevel;
    public float CurrentPoints;
    private TechLevel_SO _runtimeTechLevel;
    private bool _pended;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private List<(TechLevelUnlockEventType evtType, int id)> _pendingUnlockEvents = new();

    public void demoCall()
    {
        DataManager.Instance.SaveDynamicData(_runtimeTechLevel, "ArchiveTechLevel.json");
        EventHandler.CallTechPointChange(5);
    }

    private void OnEnable()
    {
        EventHandler.OnTechPointsChanged += AddTechPoints;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.OnTechPointsChanged -= AddTechPoints;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainScene" && scene.name != "MainMenu" && scene.name != "OpeningAnimation" && scene.name != "Thank")
        {
            Debug.Log("CurrentTechLevel" + CurrentTechLevel);
            Debug.Log(_runtimeTechLevel.techLevelData.Count);
            //如果当前等级的事件并未激活，则唤起LevelUpPanel。
            int latestLevel = CheckPendingEvent();

            if (_pended || latestLevel != 0)
            {
                UIManager.Instance.UILevelUpPanel.OpenTab();
            }
        }
    }

    public void Init()
    {
        _runtimeTechLevel = DataManager.Instance.archiveTechLevel;

        if (_runtimeTechLevel == null)
        {
            Debug.LogError("TechLevel_SO 实例未加载！");
            return;
        }

        if (_runtimeTechLevel.techLevelData == null || _runtimeTechLevel.techLevelData.Count == 0)
        {
            Debug.LogError("科技等级数据为空！");
            return;
        }


        CurrentTechLevel = _runtimeTechLevel.CurrentLevel;
        CurrentPoints = _runtimeTechLevel.CurrentPoints;

        TechLevelDetails detail;
        if (DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out detail))
        {
            UIManager.Instance.TechLevelPanel.pointsLimit = detail.needPoints;
        }

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
        if (DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out nextLevelDetail))
        {
            if (CurrentPoints >= nextLevelDetail.needPoints)
            {
                CurrentPoints -= nextLevelDetail.needPoints;
                LevelUp();


                // ??????????????????CurrentLevel??
                TechLevelDetails newLevelDetail;
                if (DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out newLevelDetail))
                {
                    UIManager.Instance.TechLevelPanel.pointsLimit = newLevelDetail.needPoints;
                }

                if (SceneManager.GetActiveScene().name == "MainScene")
                    EventHandler.CallSystemMessageShow("有些事情想在今天结束的时候考虑一下。");

                UIManager.Instance.TechLevelPanel.LevelUpUI(CurrentTechLevel, CurrentPoints);
                UIManager.Instance.UILevelUpPanel.InitLevel(CurrentTechLevel - 1, CurrentTechLevel);

                // ???????????????
                CheckLevelUp(); // ?????
            }
        }
    }
    #endregion

    public int CheckPendingEvent()
    {
        int latestLevel = 0;
        if (CurrentTechLevel >= 2)
        {
            for (int i = 2; i <= CurrentTechLevel; i++)
            {
                if (_runtimeTechLevel.EventHasTrigger(i))
                {
                    continue;
                }
                PendEvent(i-1);
                latestLevel = i;
            }
        }
        UIManager.Instance.UILevelUpPanel.InitLevel(CurrentTechLevel - 1, CurrentTechLevel);
        return latestLevel;
    }

    private void LevelUp()
    {
        CurrentTechLevel += 1;
        _runtimeTechLevel.CurrentLevel = CurrentTechLevel;

        if (_runtimeTechLevel.EventHasTrigger(CurrentTechLevel))
            return;

        CheckPendingEvent();
    }

    private void PendEvent(int level)
    {
        TechLevelEventData data;
        if (DataManager.Instance.TechLevelEventDatas.TryGetValue(level, out data))
        {
            for (int i = 0; i < data.triggerEvents.Count; i++)
            {
                if (data.triggerID.Count > i && data.triggerID[i] != 0)
                {

                    _pendingUnlockEvents.Add((data.triggerEvents[i], data.triggerID[i]));
                }
                else
                {
                }
            }

            UIManager.Instance.UILevelUpPanel.InitContent(data.triggerEvents.Count, data.triggerEvents, data.triggerID);
            _pended = true;
            _runtimeTechLevel.techLevelData[level].SetBool(true);
        }
    }

    public void TriggerPendingUnlockEvents()
    {
        foreach (var evt in _pendingUnlockEvents)
        {
            DispatchUpgradeEvent(evt.evtType, evt.id);
        }

        _pended = false;
        _pendingUnlockEvents.Clear();
    }
    private void DispatchUpgradeEvent(TechLevelUnlockEventType evt, int num)
    {
        EventHandler.CallTechLevelUpEvent(CurrentTechLevel, evt, num);
    }
}