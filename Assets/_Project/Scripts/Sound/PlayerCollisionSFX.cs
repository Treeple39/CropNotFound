using UnityEngine;

public class PlayerCollisionSFX : BaseSoundController 
{
    [Header("���ר����ײ��Ч")]
    [Tooltip("��'Slippers'(Ь��)���͵���ײ��ʱ���ŵ���Ч")]
    [SerializeField] private AudioClip hitBySlippersSound;

    [Tooltip("����'Bear'(��)���͵���ʱ���ŵ���Ч")]
    [SerializeField] private AudioClip touchBearSound;

    [Tooltip("���������κοɽ���/�ϰ���ʱ���ŵ�ͨ����Ч")]
    [SerializeField] private AudioClip genericHitSound;

    // OnCollisionEnter2D ������ҵ���ײ����������ײ�忪ʼ�Ӵ�ʱ������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���Դ���ײ�����ȡ Enemy �ű�
        Enemy enemyComponent = collision.gameObject.GetComponent<Enemy>();

        if (enemyComponent != null)
        {
            // ���ݵ������ϵ��ƶ��ű������ֵ�������
            if (collision.gameObject.GetComponent<SlippersMovement>() != null)
            {
                PlaySound(hitBySlippersSound); // ���û���Ĳ��ŷ���
            }
            else if (collision.gameObject.GetComponent<DollMovement>() != null)
            {
                PlaySound(touchBearSound);
            }
            else
            {
                PlaySound(genericHitSound);
            }
        }
        else
        {
            // ײ���ǵ��˶��󣬱���ǽ��
            if (collision.gameObject.CompareTag("Objects")|| collision.gameObject.CompareTag("Border"))
            {
                PlaySound(genericHitSound);
            }
        }
    }
}