using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : Singleton<ScoreDetector>
{
    [SerializeField] public int _lastItemCount;
    [SerializeField] public int soulAte = 2; //在这里可以设定吃多少个灵魂触发一次
    [SerializeField] public List<int> itemIdList = new List<int>(); //在这里可以设定领奖物品
    [SerializeField] private itemUITipDatabase itemUITipDatabase;

    private void Update()
    {
        // 持续检测itemCount变化
        if (Score.itemCount >= _lastItemCount + soulAte)
        {
            _lastItemCount = Score.itemCount;

            //执行抽卡
            TriggerRandomEvent();

            Debug.Log("触发了一次");
        }
    }

    //分数检测的随机事件（未来出配置需求）
    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .6f)
        {   // 0.0-0.6 (60%)
            EventHandler.CallMessageShow();
        }
        else if (randomValue < 0.9f && randomValue > 0.6f)
        {  // 0.6-0.9 (30%)
            
            int temp = Random.Range(0, itemIdList.Count);

            int selectedItemId = itemIdList[temp];
            InventoryManager.Instance.AddItem(selectedItemId);

            ItemUIData itemGet = itemUITipDatabase.GetItemUIData(selectedItemId);
            EventHandler.CallItemGet(itemGet, temp+10);
        }
        else                          // 0.9-1.0 (10%)
            EventHandler.CallBoostSpeed(2, 5);
    }
}
