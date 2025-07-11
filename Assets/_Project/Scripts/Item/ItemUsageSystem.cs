// ItemUsageSystem.cs
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ItemUsageSystem : MonoBehaviour
{
    private Dictionary<ItemType, IItemSpellUsage> _spells;

    private void Awake()
    {
        InitSpells();
    }

    private void OnEnable()
    {
        EventHandler.ItemSpellUse += OnItemUseRequested;
    }

    private void OnDisable()
    {
        EventHandler.ItemSpellUse -= OnItemUseRequested;
    }

    private void InitSpells()
    {
        _spells = new Dictionary<ItemType, IItemSpellUsage>
        {
            { ItemType.Clock, new ClockUsageStrategy() },
            { ItemType.Place, new PlacePrefabStrategy() },
            //……等我后续补充吧哈哈哈哈
        };
    }

    private void OnItemUseRequested(ItemType type, ItemDetails item)
    {
        if (_spells.TryGetValue(type, out var spell))
        {
            if (InventoryManager.Instance.CanRemoveItem(item.ID, 1))
            {
                bool success = spell.Execute(item);
                if (success)
                {
                    // 使用成功，更新背包
                    InventoryManager.Instance.RemoveItem(item.ID, 1);
                }
            }
        }
        else
        {
            Debug.LogWarning($"未实现 {type} 类型的使用逻辑");
        }
    }


    private void OnDestroy()
    {
        EventHandler.ItemSpellUse -= OnItemUseRequested;
    }
}