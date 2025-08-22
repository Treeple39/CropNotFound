using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����Json������Ϸ�ھ�̬��Դ
[System.Serializable]

public class ItemDetails : Details
{
    public string itemType { get; set; }
    public string itemRarity { get; set; }
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
public class SingleUnlockHintsData
{
    public int ID { get; set; }
    public TechLevelUnlockEventType unlockType { get; set; }
    public int unlockID { get; set; }
    public string messageText { get; set; }
    public float messageDuration { get; set; }

}

[System.Serializable]
public class TechLevelEventData
{
    public int techLevel { get; set; }
    public List<TechLevelUnlockEventType> triggerEvents { get; set; }
    public List<int> triggerID { get; set; }
    public List<int> unlockHintID { get; set; }
}

[System.Serializable]
public class TechLevelDetails
{
    public int techLevel { get; set; }
    public float needPoints { get; set; }
}

//����ScriptObject������Ϸ�ڶ�̬��Դ
[System.Serializable]
public struct InventoryItem //struct��ֵ����
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
    public int messageID;       // ��Ʒ���
    public Sprite messageImage; // ��ӦͼƬ
    public string message;   // ��Ӧ�ı�
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
    public Color contentTypeColor;
}

[System.Serializable]
public class UnlockHintData
{
    public int ID;
    public bool triggered;

    public void SetBool(bool i)
    {
        triggered = i;
    }
}

[System.Serializable]
public class FxImagePos
{
    public bool canfly;
    public Sprite image;
    public Vector3 pos;
    public RarityData Rarity;
}

[System.Serializable]
public class ShopItem
{
    public int itemId;
    public int amount;
}
