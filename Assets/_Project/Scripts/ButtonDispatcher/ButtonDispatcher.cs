using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDispatcher : Singleton<ButtonDispatcher>
{
    private Dictionary<string, Action> dispatchTable = new();

    private void OnEnable()
    {
        //按钮绑定(测试)
        Register("dash", () => InputManager.Instance.DashPressed = true);
    }


    public void Register(string key, Action callback)
    {
        if (!dispatchTable.ContainsKey(key))
            dispatchTable.Add(key, callback);
        else
            dispatchTable[key] = callback; // 允许覆盖
    }

    public bool TryGet(string key, out Action callback)
    {
        return dispatchTable.TryGetValue(key, out callback);
    }

    // 清除所有注册（用于切换场景时）
    public void Clear() => dispatchTable.Clear();
}
