using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemUITipDatabase", menuName = "Database/itemUITipDatabase")]

public class itemUITipDatabase : ScriptableObject
{
    public List<ItemUIData> ItemUIDatas;
}
