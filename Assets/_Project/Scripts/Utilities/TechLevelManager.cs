using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class TechLevelManager : Singleton<TechLevelManager>
{
    //全局科技等级
    public int CurrentTechLevel;
    public float CurrentPoints;
    private TechLevel_SO _runtimeTechLevel;

    public void demoCall()
    {
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

        // 初始化动态数据
        CurrentTechLevel = _runtimeTechLevel.CurrentLevel;
        CurrentPoints = _runtimeTechLevel.CurrentPoints;

        // 设置点数上限（索引 = CurrentLevel - 1）
        TechLevelDetails detail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel - 1, out detail))
        {
            UIManager.Instance.TechLevelPanel.pointsLimit = detail.needPoints;
        }

        // 初始化UI显示
        UIManager.Instance.TechLevelPanel.InitTechLevelUI(CurrentTechLevel, CurrentPoints);
    }

    #region PointsGain
    public void AddTechPoints(float amount)
    {
        CurrentPoints += amount;
        UIManager.Instance.TechLevelPanel.AddPointsUI(CurrentPoints);

        CheckLevelUp();
        _runtimeTechLevel.CurrentPoints = CurrentPoints;

        // 每次获得经验均自动存储数据
        DataManager.Instance.archiveTechLevel = _runtimeTechLevel;
        DataManager.Instance.SaveDynamicData(_runtimeTechLevel, "ArchiveTechLevel.json");
    }

    private void CheckLevelUp()
    {
        // 获取下一级的信息（索引 = CurrentLevel，因为0对应1级）
        TechLevelDetails nextLevelDetail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out nextLevelDetail))
        {
            if(CurrentPoints >= nextLevelDetail.needPoints)
            {
                CurrentPoints -= nextLevelDetail.needPoints;
                LevelUp();
                
                
                // 设置新的点数上限（新的CurrentLevel）
                TechLevelDetails newLevelDetail;
                if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out newLevelDetail))
                {
                    UIManager.Instance.TechLevelPanel.pointsLimit = newLevelDetail.needPoints;
                }
                // 更新UI显示
                UIManager.Instance.TechLevelPanel.LevelUpUI(CurrentTechLevel, CurrentPoints);

                // 检查是否还能继续升级
                CheckLevelUp(); // 递归检查
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

        // 触发事件（索引 = CurrentLevel - 1）
        TechLevelEventData data;
        if (DataManager.Instance.TechLevelEventDatas.TryGetValue(CurrentTechLevel - 1, out data))
        {
            for (int i = 0; i < data.triggerEvents.Count; i++) 
            {
                if(data.triggerID.Count > i && data.triggerID[i] != 0)
                {
                    DispatchUpgradeEvent(data.triggerEvents[i], data.triggerID[i]);
                }
                else
                {
                    Debug.LogWarning($"事件{i}未正确配置triggerID");
                }
            }

            _runtimeTechLevel.techLevelData[CurrentTechLevel - 1].SetBool(true);
        }
    }

    private void DispatchUpgradeEvent(TechLevelUnlockEventType evt, int num)
    {
        EventHandler.CallTechLevelUpEvent(CurrentTechLevel, evt, num);
    }
}