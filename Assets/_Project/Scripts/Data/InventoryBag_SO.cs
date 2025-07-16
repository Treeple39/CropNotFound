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
    }
}
