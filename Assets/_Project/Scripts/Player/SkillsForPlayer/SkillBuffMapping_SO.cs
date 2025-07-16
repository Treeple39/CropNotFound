using System;
using System.Collections.Generic;
using System.Linq; // 引入LINQ以便使用更简洁的代码
using UnityEngine;

// [Serializable] 让这个类的实例可以在Inspector中被编辑
// 我们将原来的 SkillBuffEntry 扩展为包含所有技能信息
[Serializable]
public class SkillBuffEntry
{
    [Tooltip("技能的唯一ID")]
    public int skillID;

    [Tooltip("技能的显示名称")]
    public string skillName;

    [Tooltip("技能图标在Resources文件夹下的路径")]
    public string iconPath;

    [Tooltip("技能的详细描述")]
    [TextArea(3, 5)] // 让描述字段在Inspector里可以输入多行
    public string description;

    [Tooltip("此技能效果对应的Buff脚本文件名（无需.cs后缀）")]
    public string buffScriptName;

    [Tooltip("技能的类型，用于可能的逻辑分类")]
    public string skillType;
}

[CreateAssetMenu(fileName = "SkillBuffMapping", menuName = "Skills/Skill Buff Mapping")]
public class SkillBuffMapping_SO : ScriptableObject
{
    public List<SkillBuffEntry> skillBuffs;

    // 缓存字典，用于快速查找 ID -> Buff脚本 的映射关系
    private Dictionary<int, string> _mappingCache;

    /// <summary>
    /// 获取技能ID到Buff脚本名称的映射字典。
    /// BuffApplicationManager会使用这个方法。
    /// </summary>
    public Dictionary<int, string> GetMapping()
    {
        if (_mappingCache == null)
        {
            _mappingCache = skillBuffs
                .Where(entry => !string.IsNullOrEmpty(entry.buffScriptName))
                .ToDictionary(entry => entry.skillID, entry => entry.buffScriptName);
        }
        return _mappingCache;
    }
    public SkillBuffEntry GetSkillEntryByID(int skillID)
    {
        // 使用LINQ的FirstOrDefault方法来查找匹配的条目
        return skillBuffs.FirstOrDefault(entry => entry.skillID == skillID);
    }
}