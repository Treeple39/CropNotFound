using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockHint_SO", menuName = "PlayerData/UnlockHint_SO")]

public class UnlockHint_SO : ScriptableObject, IInitializableSO
{
    public int ID;
    public List<UnlockHintData> unlockHintData;

    public void InitDefault()
    {
        unlockHintData.Clear();

        for (int i = 0; i < 100; i++) // 初始化1-100级，每级默认未解锁
        {
            unlockHintData. Add(new UnlockHintData { ID = i + 14000, triggered = false });
            unlockHintData[i].SetBool(false);
        }
    }
}