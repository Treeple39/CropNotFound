using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryBag_SO")]

public class InventoryBag_SO : ScriptableObject, IInitializableSO
{
    public List<InventoryItem> itemList;

    public void InitDefault()
    {
        itemList.Clear();
        for (int i = 0; i < 3; i++) // 初始化1-100级，每级默认未解锁
        {
            itemList.Add(new InventoryItem { itemID = default, itemAmount = 0 });
        }
    }
}
