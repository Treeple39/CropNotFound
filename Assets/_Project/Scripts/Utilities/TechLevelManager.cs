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

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    //�¼����������
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
                // ����UI�׼�����Ӧ��·����
                EventHandler.CallSystemMessageShow("��Щ�������ڽ��������ʱ����һ�¡���");
                UIManager.Instance.TechLevelPanel.LevelUpUI(CurrentTechLevel, CurrentPoints);
                UIManager.Instance.UILevelUpPanel.InitLevel(CurrentTechLevel - 1, CurrentTechLevel);

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

        TechLevelEventData data;
        if (DataManager.Instance.TechLevelEventDatas.TryGetValue(CurrentTechLevel - 1, out data))
        {
            _pendingUnlockEvents.Clear();

            for (int i = 0; i < data.triggerEvents.Count; i++)
            {
                if (data.triggerID.Count > i && data.triggerID[i] != 0)
                {

                    //�ݴ��¼��������ȷ�Ͻ���ʱ�ٴ���
                    _pendingUnlockEvents.Add((data.triggerEvents[i], data.triggerID[i]));
                }
                else
                {
                    Debug.LogWarning($"�¼�{i}δ��ȷ����triggerID, ��ʹ��Ĭ��ID");
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