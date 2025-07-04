using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public interface IItemSpellUsage
{
    bool Execute(ItemDetails itemDetails); // 返回是否使用成功
}