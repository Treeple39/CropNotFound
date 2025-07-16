using System;
using System.Collections.Generic;
using System.Linq; // ����LINQ�Ա�ʹ�ø����Ĵ���
using UnityEngine;

// [Serializable] ��������ʵ��������Inspector�б��༭
// ���ǽ�ԭ���� SkillBuffEntry ��չΪ�������м�����Ϣ
[Serializable]
public class SkillBuffEntry
{
    [Tooltip("���ܵ�ΨһID")]
    public int skillID;

    [Tooltip("���ܵ���ʾ����")]
    public string skillName;

    [Tooltip("����ͼ����Resources�ļ����µ�·��")]
    public string iconPath;

    [Tooltip("���ܵ���ϸ����")]
    [TextArea(3, 5)] // �������ֶ���Inspector������������
    public string description;

    [Tooltip("�˼���Ч����Ӧ��Buff�ű��ļ���������.cs��׺��")]
    public string buffScriptName;

    [Tooltip("���ܵ����ͣ����ڿ��ܵ��߼�����")]
    public string skillType;
}

[CreateAssetMenu(fileName = "SkillBuffMapping", menuName = "Skills/Skill Buff Mapping")]
public class SkillBuffMapping_SO : ScriptableObject
{
    public List<SkillBuffEntry> skillBuffs;

    // �����ֵ䣬���ڿ��ٲ��� ID -> Buff�ű� ��ӳ���ϵ
    private Dictionary<int, string> _mappingCache;

    /// <summary>
    /// ��ȡ����ID��Buff�ű����Ƶ�ӳ���ֵ䡣
    /// BuffApplicationManager��ʹ�����������
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
        // ʹ��LINQ��FirstOrDefault����������ƥ�����Ŀ
        return skillBuffs.FirstOrDefault(entry => entry.skillID == skillID);
    }
}