using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//采用Json管理游戏内静态资源
[System.Serializable]

public class ItemDetails
{
    public int itemID { get; set; }
    public string itemName { get; set; }
    public string itemType { get; set; }
    public string itemIcon { get; set; }
    public string itemSpriteOnWorld { get; set; }
    public string itemDescription { get; set; }
    public float itemUseRadius { get; set; }
    public bool canPickedup { get; set; }
    public bool canDropped { get; set; }
    public bool canCarried { get; set; }
    public bool canUseSpell { get; set; }
    public string prefabToSpawnPath { get; set; }
}

//采用ScriptObject管理游戏内动态资源
[System.Serializable]
public struct InventoryItem //struct：值类型
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public struct RarityData
{
    public Rarity rarity;
    public Color outlineColor;
    public float outlineWidth;
    public float statMultiplier;
    public string displayName;
}

[System.Serializable]
public struct ItemUIData   //这个是为60%的随机事件做的，物体的在上面那个预制体表加。然后需要做成掉落，就加预制体，如果是直接增加数量就改下面的函数
{
    public int messageID;       // 物品编号
    public Sprite messageImage; // 对应图片
    public string message;   // 对应文本
}