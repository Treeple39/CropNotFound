using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDispatcher : Singleton<ButtonDispatcher>
{
    private Dictionary<string, Action> dispatchTable = new();

    private void OnEnable()
    {
        Register("dash", () => InputManager.Instance.DashPressed = true);
    }


    public void Register(string key, Action callback)
    {
        if (!dispatchTable.ContainsKey(key))
            dispatchTable.Add(key, callback);
        else
            dispatchTable[key] = callback; // ��������
    }

    public bool TryGet(string key, out Action callback)
    {
        return dispatchTable.TryGetValue(key, out callback);
    }

    // �������ע�ᣨ�����л�����ʱ��
    public void Clear() => dispatchTable.Clear();
}
