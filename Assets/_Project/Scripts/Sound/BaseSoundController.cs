using UnityEngine;

// ȷ���κμ̳д˽ű������������Ϸ�����϶���һ��AudioSource
[RequireComponent(typeof(AudioSource))]
public abstract class BaseSoundController : MonoBehaviour
{
    [Header("ͨ����Ч����")]
    [Tooltip("�����ܴ����������롣����������룬��������ȫ��������")]
    [SerializeField] protected float maxSoundDistance = 20f;

    [Tooltip("����������ڣ����������������")]
    [SerializeField] protected float minSoundDistance = 1f;

    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        // ��ȡAudioSource���
        audioSource = GetComponent<AudioSource>();

        // --- ���ģ����ÿռ���Ч ---
        // �� spatialBlend ����Ϊ 1����������һ����ȫ��3D��Ч���������������˥��
        audioSource.spatialBlend = 1.0f;

        // ��������˥����ģʽ��Logarithmic (����) ������Ȼ��ģʽ
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        // ����������С����
        audioSource.minDistance = minSoundDistance;
        audioSource.maxDistance = maxSoundDistance;
    }

    /// <summary>
    /// �ܱ����Ĳ��ŷ�����ֻ��������Ե���
    /// </summary>
    /// <param name="clipToPlay">Ҫ���ŵ���ЧƬ��</param>
    protected void PlaySound(AudioClip clipToPlay)
    {
        if (clipToPlay != null)
        {
            // ʹ�� PlayOneShot �����ڲ������������������²�����Ч
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning($"��ͼ�� {gameObject.name} �ϲ���һ���յ���ЧƬ�Σ�", this);
        }
    }
}