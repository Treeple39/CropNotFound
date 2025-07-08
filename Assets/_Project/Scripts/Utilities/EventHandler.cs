using Inventory;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static event Action<ItemType, ItemDetails> ItemSpellUse;
    public static event Action<SlotUI, float> OnCooldownStart;
    public static event Action<SlotUI> OnCooldownEnd;
    public static event Action<Rarity> OnRarityChanged;
    public static event Action<int, float> OnRarityUpgraded;
    public static event Action<float, float> OnChangeSpeed; //速度调整，时长
    public static event Action<ItemUIData, float> OnMessageShow;
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

    public static void CallBoostSpeed(float speedBoostMultiplier, float speedBoostDuration)
    {
        OnChangeSpeed?.Invoke(speedBoostMultiplier, speedBoostDuration);
    }

    public static void CallMessageShow(ItemUIData itemUIData = default, float duration = 2.0f)
    {
        OnMessageShow?.Invoke(itemUIData, duration);
    }
}
