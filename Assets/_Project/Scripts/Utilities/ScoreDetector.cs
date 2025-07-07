using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : Singleton<ScoreDetector>
{
    [SerializeField] public int _lastItemCount;
    [SerializeField] public int soulAte = 2; //����������趨�Զ��ٸ���괥��һ��
    [SerializeField] public List<int> itemIdList = new List<int>(); //����������趨�콱��Ʒ

    private void Update()
    {
        // �������itemCount�仯
        if (Score.itemCount >= _lastItemCount + soulAte)
        {
            _lastItemCount = Score.itemCount;

            //ִ�г鿨
            TriggerRandomEvent();

            Debug.Log("������һ��");
        }
    }

    //������������¼���δ������������
    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .6f)       // 0.0-0.6 (60%)
            EventHandler.CallMessageShow();
        else if (randomValue < 0.9f && randomValue > 0.6f)  // 0.6-0.9 (30%)
            InventoryManager.Instance.AddItem(itemIdList[Random.Range(0, itemIdList.Count)]);
        else                          // 0.9-1.0 (10%)
            EventHandler.CallBoostSpeed(2, 5);
    }
}
