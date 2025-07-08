using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemUITipDatabase", menuName = "Database/itemUITipDatabase")]

public class itemUITipDatabase : ScriptableObject
{
  
    public List<ItemUIData> ItemUIDatas;
    
    private Dictionary<int, int> itemIdToMessageIdMap = new Dictionary<int, int>()
    {
        {1000, 8},  
        {1001, 9},  
        {1002, 10}  
    };

    public ItemUIData GetItemUIData(int itemId)
    {
        itemIdToMessageIdMap.TryGetValue(itemId, out int messageId);
        return ItemUIDatas.Find(data => data.messageID == messageId);
        
    }
}
