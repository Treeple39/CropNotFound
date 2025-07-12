using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : Singleton<ScoreDetector>
{
    [SerializeField] public int _lastItemCount;
    [SerializeField] public int soulAte = 2; //����������趨�Զ��ٸ���괥��һ��
    [SerializeField] private itemUITipDatabase itemUITipDatabase;
    [SerializeField] private TechUnlockProgess_SO techUnlockSO;//����������趨�콱��Ʒ

    private void OnEnable()
    {
        EventHandler.OnTechLevelUpEvent += UnlockItem;
    }

    private void OnDisable()
    {
        EventHandler.OnTechLevelUpEvent -= UnlockItem;
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

    /// <summary>
    /// ����д��������ķ���
    /// </summary>
    //private void UnlockEnemy(int techLevel, TechLevelUnlockEventType eventType, int num)
    //{
    //    if (eventType == TechLevelUnlockEventType.UnlockMonster)
    //    {
    //        EnemyList.Add(num);
    //    }
    //}

    public void Init()
    {
        UIManager.Instance.InitMessageUI(itemUITipDatabase);
        techUnlockSO = DataManager.Instance.techUnlockProgess;
    }

    private void Update()
    {
        // �������itemCount�仯
        if (Score.itemCount >= _lastItemCount + soulAte)
        {
            _lastItemCount = Score.itemCount;

            //ִ�г鿨
            TriggerRandomEvent();

            Debug.Log("����");
        }
    }

    //������������¼���δ������������
    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .1f)
        {   // 0.0-0.6 (60%)
            EventHandler.CallMessageShow();
        }
        else if (randomValue < 0.2f && randomValue > 0.1f)
        {  // 0.6-0.9 (30%)
            
            int temp = Random.Range(0, techUnlockSO.unlockedItemIDs.Count);

            int selectedItemId = techUnlockSO.unlockedItemIDs[temp];
            InventoryManager.Instance.AddItem(selectedItemId);

            ItemUIData itemGet = itemUITipDatabase.GetItemUIData(selectedItemId);
            EventHandler.CallItemGet(itemGet, temp+10);
        }
        else                          // 0.9-1.0 (10%)
            EventHandler.CallBoostSpeed(2, 5);
    }
}
