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