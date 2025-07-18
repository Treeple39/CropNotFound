using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : Singleton<ScoreDetector>
{
    [SerializeField] public int _lastItemCount;
    [SerializeField] public int soulAte = 2; 
    [SerializeField] private itemUITipDatabase itemUITipDatabase;
    [SerializeField] private TechUnlockProgess_SO techUnlockSO;

    public void Init()
    {
        UIManager.Instance.InitMessageUI(itemUITipDatabase);
        techUnlockSO = DataManager.Instance.techUnlockProgess;
    }

    private void Update()
    {
        if (Score.itemCount >= _lastItemCount + soulAte)
        {
            _lastItemCount = Score.itemCount;

            TriggerRandomEvent();

            Debug.Log("????");
        }
    }

    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .6f)
        {   // 0.0-0.6 (60%)
            EventHandler.CallMessageShow();
        }
        else if (randomValue < 0.9f && randomValue > 0.6f && techUnlockSO.unlockedItemIDs.Count > 0)
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
