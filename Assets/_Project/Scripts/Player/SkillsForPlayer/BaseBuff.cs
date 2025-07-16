using UnityEngine;

// 抽象基类，所有具体的Buff脚本继承
public abstract class BaseBuff : MonoBehaviour
{
    // Buff的名称
    public string buffName;

    // 当Buff被应用时调用的方法
    // target 是被施加Buff的游戏对象
    public abstract void ApplyBuff(GameObject target);

}