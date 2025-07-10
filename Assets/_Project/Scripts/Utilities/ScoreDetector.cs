using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : Singleton<ScoreDetector>
{
    [SerializeField] public int _lastItemCount;
    [SerializeField] public int soulAte = 2; //在这里可以设定吃多少个灵魂触发一次
    [SerializeField] private itemUITipDatabase itemUITipDatabase;
    [SerializeField] private TechUnlockProgess_SO techUnlockSO;//在这里可以设定领奖物品

    private void OnEnable()
    {
        EventHandler.OnTechLevelUpEvent += UnlockItem;
    }

    private void OnDisable()
    {
        EventHandler.OnTechLevelUpEvent -= UnlockItem;
    }

    //TODO:三种解锁逻辑暂不整合，有空再做吧
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
    /// 这里写解锁怪物的方法
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
    }

    private void Update()
    {
        // 持续检测itemCount变化
        if (Score.itemCount >= _lastItemCount + soulAte)
        {
            _lastItemCount = Score.itemCount;

            //执行抽卡
            TriggerRandomEvent();

            Debug.Log("触发");
        }
    }

    //分数检测的随机事件（未来出配置需求）
    private void TriggerRandomEvent()
    {
        if (techUnlockSO.unlockedItemIDs.Count == 0)
            Debug.LogError("你好大的胆子，居然敢不配置SO！");
            return;

        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .6f)
        {   // 0.0-0.6 (60%)
            EventHandler.CallMessageShow();
        }
        else if (randomValue < 0.9f && randomValue > 0.6f)
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
