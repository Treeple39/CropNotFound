using System.Collections.Generic;
using UnityEngine;

public class BuffApplicationManager : Singleton<BuffApplicationManager> // �Ƽ�Ҳ���ɵ���
{
    [Header("��������")]
    [SerializeField] private TechUnlockProgess_SO techUnlockProgress; // ��Ϊ������� SO
    [SerializeField] private SkillBuffMapping_SO skillBuffMapping;   // ����ID->Buff�ű���ӳ��

    [Header("Ŀ�����")]
    [SerializeField] private GameObject playerObject; // ��Ҫ��ʩ��Buff����Ҷ���

    /// <summary>
    /// ����Ϸ������ɣ�������ݺͶ���׼���ú����
    /// </summary>
    public void ApplyAllUnlockedBuffs()
    {
        if (techUnlockProgress == null || skillBuffMapping == null || playerObject == null)
        {
            Debug.LogError("BuffӦ�ù�����ȱ�ٱ�Ҫ�����ã�����Inspector�и�ֵ��");
            return;
        }

        var mappingDict = skillBuffMapping.GetMapping();

        Debug.Log($"��ʼӦ���ѽ����� {techUnlockProgress.unlockedSkillIDs.Count} ������Buff...");

        // ֱ�ӱ�����SO�е�unlockedSkillIDs�б�
        foreach (int skillId in techUnlockProgress.unlockedSkillIDs)
        {
            ApplySingleBuff(skillId, mappingDict);
        }
    }

    /// <summary>
    /// Ӧ�õ���Buff�����Ա��ⲿ�����¼��ܽ���ʱ������
    /// </summary>
    /// <param name="skillId">ҪӦ�õļ���ID</param>
    public void ApplySingleBuff(int skillId)
    {
        // ����һ�����������ط����������ⲿ����
        ApplySingleBuff(skillId, skillBuffMapping.GetMapping());
    }
    private void ApplySingleBuff(int skillId, Dictionary<int, string> mappingDict)
    {
        if (mappingDict.TryGetValue(skillId, out string scriptName))
        {
            // �������Ƿ��Ѿ�ӵ�����Buff�������ֹ�ظ����
            // ע�⣺GetComponent(string) ���ܽϵͣ���ֻ�ڼ��غͽ���ʱ���ã���ȫ���Խ���
            if (playerObject.GetComponent(scriptName) == null)
            {
                Debug.Log($"Ϊ������Buff������ID: {skillId}, �ű�: {scriptName}");

                // ��̬�����������Ǻ���
                Component newBuffComponent = playerObject.AddComponent(System.Type.GetType(scriptName));

                // ����Buff��Apply������ʩ��Ч��
                if (newBuffComponent is BaseBuff buff)
                {
                    buff.ApplyBuff(playerObject);
                }
                else
                {
                    Debug.LogWarning($"�ű� {scriptName} ����һ����Ч��BaseBuff���޷�����ApplyBuff��");
                }
            }
        }
    }
}