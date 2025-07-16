using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//采用Json管理游戏内静态资源
[System.Serializable]

public class ItemDetails : Details
{
    public string itemType { get; set; }
    public string itemSpriteOnWorld { get; set; }
    public float itemUseRadius { get; set; }
    public bool canPickedup { get; set; }
    public bool canDropped { get; set; }
    public bool canCarried { get; set; }
    public bool canUseSpell { get; set; }
    public string prefabToSpawnPath { get; set; }
}

public class EnemyDetails : Details
{
    public string EnemyType { get; set; }
    public string moveMod { get; set; }
    public string prefabToSpawnPath { get; set; }
}

public class SkillDetails : Details
{
    public string skillType { get; set; }
}

[System.Serializable]
public class RoomData
{
    public string roomName { get; set; }
}




[System.Serializable]
public class TechLevelEventData
{
    public int techLevel { get; set; }
    public List<TechLevelUnlockEventType> triggerEvents { get; set; }
    public List<int> triggerID { get; set; }
}

[System.Serializable]
public class TechLevelDetails
{
    public int techLevel { get; set; }
    public float needPoints { get; set; }
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
public class ItemUIData   
{
    public int messageID;       // 物品编号
    public Sprite messageImage; // 对应图片
    public string message;   // 对应文本
}

[System.Serializable]
public class TechLevelData
{
    public int techLevel;
    public bool techLevelEventHasTrigger;

    public void SetBool(bool i)
    {
        techLevelEventHasTrigger = i;
    }
}

[System.Serializable]
public class TechUnlockSaveData
{
    public List<int> unlockedItemIDs;
    public List<int> unlockedMonsterIDs;
    public List<int> unlockedSkillIDs;
}

[System.Serializable]
public class LevelUpContentData
{
    public string contentText;
    public string contentTitle;
    public string contentTypeTip;
    public Sprite contentImage;
}

[System.Serializable]
public class SingleUnlockHintsData
{
    public TechLevelUnlockEventType unlockType;
    public bool triggered;
    public string messageText;

    public void SetBool(bool i)
    {
        triggered = i;
    }
}

[System.Serializable]
public class UnlockHintEntry
{
    public int key;
    public SingleUnlockHintsData value;
}

