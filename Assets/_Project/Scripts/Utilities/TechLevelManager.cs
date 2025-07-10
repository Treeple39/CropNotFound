using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class TechLevelManager : Singleton<TechLevelManager>
{
    //ȫ�ֿƼ��ȼ�
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

        // ��ʼ����̬����
        CurrentTechLevel = _runtimeTechLevel.CurrentLevel;
        CurrentPoints = _runtimeTechLevel.CurrentPoints;

        // ���õ������ޣ����� = CurrentLevel - 1��
        TechLevelDetails detail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel - 1, out detail))
        {
            UIManager.Instance.TechLevelPanel.pointsLimit = detail.needPoints;
        }

        // ��ʼ��UI��ʾ
        UIManager.Instance.TechLevelPanel.InitTechLevelUI(CurrentTechLevel, CurrentPoints);
    }

    #region PointsGain
    public void AddTechPoints(float amount)
    {
        CurrentPoints += amount;
        UIManager.Instance.TechLevelPanel.AddPointsUI(CurrentPoints);

        CheckLevelUp();
        _runtimeTechLevel.CurrentPoints = CurrentPoints;

        // ÿ�λ�þ�����Զ��洢����
        DataManager.Instance.archiveTechLevel = _runtimeTechLevel;
        DataManager.Instance.SaveDynamicData(_runtimeTechLevel, "ArchiveTechLevel.json");
    }

    private void CheckLevelUp()
    {
        // ��ȡ��һ������Ϣ������ = CurrentLevel����Ϊ0��Ӧ1����
        TechLevelDetails nextLevelDetail;
        if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out nextLevelDetail))
        {
            if(CurrentPoints >= nextLevelDetail.needPoints)
            {
                CurrentPoints -= nextLevelDetail.needPoints;
                LevelUp();
                
                
                // �����µĵ������ޣ��µ�CurrentLevel��
                TechLevelDetails newLevelDetail;
                if(DataManager.Instance.TechLevelDetails.TryGetValue(CurrentTechLevel, out newLevelDetail))
                {
                    UIManager.Instance.TechLevelPanel.pointsLimit = newLevelDetail.needPoints;
                }
                // ����UI��ʾ
                UIManager.Instance.TechLevelPanel.LevelUpUI(CurrentTechLevel, CurrentPoints);

                // ����Ƿ��ܼ�������
                CheckLevelUp(); // �ݹ���
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

        // �����¼������� = CurrentLevel - 1��
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
                    Debug.LogWarning($"�¼�{i}δ��ȷ����triggerID");
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