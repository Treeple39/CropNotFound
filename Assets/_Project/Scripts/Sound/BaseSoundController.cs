using UnityEngine;

// 确保任何继承此脚本的组件，其游戏对象上都有一个AudioSource
[RequireComponent(typeof(AudioSource))]
public abstract class BaseSoundController : MonoBehaviour
{
    [Header("通用音效设置")]
    [Tooltip("声音能传播的最大距离。超过这个距离，声音将完全听不见。")]
    [SerializeField] protected float maxSoundDistance = 20f;

    [Tooltip("在这个距离内，声音是最大音量。")]
    [SerializeField] protected float minSoundDistance = 1f;

    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();

        // --- 核心：设置空间音效 ---
        // 将 spatialBlend 设置为 1，代表这是一个完全的3D音效，其音量会随距离衰减
        audioSource.spatialBlend = 1.0f;

        // 设置声音衰减的模式，Logarithmic (对数) 是最自然的模式
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        // 设置最大和最小距离
        audioSource.minDistance = minSoundDistance;
        audioSource.maxDistance = maxSoundDistance;
    }

    /// <summary>
    /// 受保护的播放方法，只有子类可以调用
    /// </summary>
    /// <param name="clipToPlay">要播放的音效片段</param>
    protected void PlaySound(AudioClip clipToPlay)
    {
        if (clipToPlay != null)
        {
            // 使用 PlayOneShot 可以在不打断其他声音的情况下播放音效
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning($"试图在 {gameObject.name} 上播放一个空的音效片段！", this);
        }
    }
}