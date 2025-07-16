using UnityEngine;

public class MonsterCollisionSFX : BaseSoundController // ͬ���̳��Ի���
{
    [Header("����ר����ײ��Ч")]
    [Tooltip("����ײ��ǽ�ڻ������ϰ���ʱ���ŵ���Ч")]
    [SerializeField] private AudioClip bumpSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �����߼������ײ��������**����**���
        if (!collision.gameObject.CompareTag("Player"))
        {
            // Ϊ�˱������֮�以����ײҲ�������������Խ�һ���޶�ֻ��ײǽʱ��
            if (collision.gameObject.CompareTag("Wall"))
            {
                PlaySound(bumpSound); // ���û���Ĳ��ŷ���
            }
        }
    }
}