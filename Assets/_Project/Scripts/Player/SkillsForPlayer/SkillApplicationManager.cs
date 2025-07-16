using System.Collections.Generic;
using UnityEngine;

public class BuffApplicationManager : Singleton<BuffApplicationManager> // 推荐也做成单例
{
    [Header("数据引用")]
    [SerializeField] private TechUnlockProgess_SO techUnlockProgress; // 改为引用你的 SO
    [SerializeField] private SkillBuffMapping_SO skillBuffMapping;   // 技能ID->Buff脚本的映射

    [Header("目标对象")]
    [SerializeField] private GameObject playerObject; // 需要被施加Buff的玩家对象

    /// <summary>
    /// 在游戏加载完成，玩家数据和对象都准备好后调用
    /// </summary>
    public void ApplyAllUnlockedBuffs()
    {
        if (techUnlockProgress == null || skillBuffMapping == null || playerObject == null)
        {
            Debug.LogError("Buff应用管理器缺少必要的引用！请在Inspector中赋值。");
            return;
        }

        var mappingDict = skillBuffMapping.GetMapping();

        Debug.Log($"开始应用已解锁的 {techUnlockProgress.unlockedSkillIDs.Count} 个技能Buff...");

        // 直接遍历你SO中的unlockedSkillIDs列表
        foreach (int skillId in techUnlockProgress.unlockedSkillIDs)
        {
            ApplySingleBuff(skillId, mappingDict);
        }
    }

    /// <summary>
    /// 应用单个Buff，可以被外部（如新技能解锁时）调用
    /// </summary>
    /// <param name="skillId">要应用的技能ID</param>
    public void ApplySingleBuff(int skillId)
    {
        // 这是一个公开的重载方法，方便外部调用
        ApplySingleBuff(skillId, skillBuffMapping.GetMapping());
    }
    private void ApplySingleBuff(int skillId, Dictionary<int, string> mappingDict)
    {
        if (mappingDict.TryGetValue(skillId, out string scriptName))
        {
            // 检查玩家是否已经拥有这个Buff组件，防止重复添加
            // 注意：GetComponent(string) 性能较低，但只在加载和解锁时调用，完全可以接受
            if (playerObject.GetComponent(scriptName) == null)
            {
                Debug.Log($"为玩家添加Buff，技能ID: {skillId}, 脚本: {scriptName}");

                // 动态添加组件！这是核心
                Component newBuffComponent = playerObject.AddComponent(System.Type.GetType(scriptName));

                // 调用Buff的Apply方法来施加效果
                if (newBuffComponent is BaseBuff buff)
                {
                    buff.ApplyBuff(playerObject);
                }
                else
                {
                    Debug.LogWarning($"脚本 {scriptName} 不是一个有效的BaseBuff，无法调用ApplyBuff。");
                }
            }
        }
    }
}