using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����Json������Ϸ�ھ�̬��Դ
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
public struct ItemUIData   //�����Ϊ60%������¼����ģ�������������Ǹ�Ԥ�����ӡ�Ȼ����Ҫ���ɵ��䣬�ͼ�Ԥ���壬�����ֱ�����������͸�����ĺ���
{
    public int messageID;       // ��Ʒ���
    public Sprite messageImage; // ��ӦͼƬ
    public string message;   // ��Ӧ�ı�
}