using Inventory;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public static class EventHandler
{
    public static event Action<InputMode> OnInputModeChanged;
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static event Action<ItemType, ItemDetails> ItemSpellUse;
    public static event Action<SlotUI, float> OnCooldownStart;
    public static event Action<SlotUI> OnCooldownEnd;
    public static event Action<Rarity> OnRarityChanged;
    public static event Action<int, float> OnRarityUpgraded;
    public static event Action<float, float> OnChangeSpeed; //�ٶȵ�����ʱ��
    public static event Action<ItemUIData, float> OnMessageShow;
    public static event Action<string, float> OnSystemMessageShow;
    public static event Action<int, TechLevelUnlockEventType, int> OnTechLevelUpEvent;
    public static event Action<float> OnTechPointsChanged;
    public static event Action<int> OnNewArchiveItemGet;
    public static event Action<bool> OnArchivePanelStateChanged;

    public static void CallInputModeChanged(InputMode inputMode)
    {
        OnInputModeChanged?.Invoke(inputMode);
    }
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    public static void CallItemSpellUse(ItemType type, ItemDetails itemDetail)
    {
        ItemSpellUse?.Invoke(type, itemDetail);
    }

    public static void CallCooldownStart(SlotUI slot, float duration)
    {
        OnCooldownStart?.Invoke(slot, duration);
    }

    public static void CallCooldownEnd(SlotUI slot)
    {
        OnCooldownEnd?.Invoke(slot);
    }

    public static void CallRarityChange(Rarity rarity)
    {
        OnRarityChanged?.Invoke(rarity);
    }

    public static void CallRarityUpgrade(int rank, float rate)
    {
        OnRarityUpgraded?.Invoke(rank, rate);
    }

    public static void CallBoostSpeed(float speedBoostMultiplier, float speedBoostDuration = 5.0f)
    {
        OnChangeSpeed?.Invoke(speedBoostMultiplier, speedBoostDuration);
    }

    public static void CallMessageShow(ItemUIData itemUIData = default, float duration = 2.0f)
    {
        OnMessageShow?.Invoke(itemUIData, duration);
    }

    public static void CallSystemMessageShow(string text, float duration = 2.0f)
    {
        OnSystemMessageShow?.Invoke(text, duration);
    }

    public static void CallItemGet(ItemUIData itemUIData, float duration = 3.0f)
    {
        OnMessageShow?.Invoke(itemUIData, duration);
    }

    public static void CallTechPointChange(float points)
    {
        OnTechPointsChanged?.Invoke(points);
    }

    public static void CallTechLevelUpEvent(int newLevel, TechLevelUnlockEventType evnetType, int num)
    {
        OnTechLevelUpEvent?.Invoke(newLevel, evnetType, num);
    }

    public static void CallNewArchiveGetEvent(int archiveID)
    {
        OnNewArchiveItemGet?.Invoke(archiveID);
    }
    public static void CallArchivePanelStateChanged(bool isOpen)
    {
        OnArchivePanelStateChanged?.Invoke(isOpen);
    }
}
