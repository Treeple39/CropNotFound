using UnityEngine;

// ������࣬���о����Buff�ű��̳�
public abstract class BaseBuff : MonoBehaviour
{
    // Buff������
    public string buffName;

    // ��Buff��Ӧ��ʱ���õķ���
    // target �Ǳ�ʩ��Buff����Ϸ����
    public abstract void ApplyBuff(GameObject target);

}