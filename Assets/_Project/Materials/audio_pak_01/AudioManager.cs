using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

// ★★★ SequencedBgm 类定义放在这里，或者放在它自己的文件里 ★★★
// 将它从 MonoBehaviour 继承中解放出来
[System.Serializable]
public class SequencedBgm
{
    [Tooltip("开头部分，只播放一次")]
    public AudioClip introClip;

    [Tooltip("主题循环部分，会重复播放")]
    public AudioClip themeClip;

    [Tooltip("衔接部分，在主题之后播放，然后会再回到主题")]
    public AudioClip bridgeClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager S;

    [Header("BGM 自动播放配置")]
    [Tooltip("如果只想播放单曲循环，请填写此项")]
    public AudioClip singleBgmToPlay;
    [Tooltip("如果想播放序列音乐，请配置此项 (优先级高于单曲)")]
    public SequencedBgm sequencedBgmToPlay;

    [Header("音频源 (自动获取)")]
    [HideInInspector] public AudioSource bgmSource;
    [HideInInspector] public AudioSource fxSource;
    [HideInInspector] public AudioSource vocalSource;
    private AudioMixer mixer;
    private Coroutine bgmCoroutine;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mixer = Resources.Load<AudioMixer>("MainAudioMixer");
        bgmSource = MakeSource("BGM");
        fxSource = MakeSource("FX");
        vocalSource = MakeSource("Vocal");
    }

    // ★★★ 核心修改：在Start方法中自动播放配置好的音乐 ★★★
    void Start()
    {
        // 优先播放序列音乐
        if (sequencedBgmToPlay != null && sequencedBgmToPlay.themeClip != null)
        {
            PlaySequencedBGM(sequencedBgmToPlay);
        }
        // 如果没有配置序列音乐，再检查是否配置了单曲音乐
        else if (singleBgmToPlay != null)
        {
            PlayBGM(singleBgmToPlay);
        }
    }

    AudioSource MakeSource(string groupName)
    {
        var src = gameObject.AddComponent<AudioSource>();
        var group = mixer.FindMatchingGroups(groupName);
        if (group.Length > 0) src.outputAudioMixerGroup = group[0];
        return src;
    }

    // --- 播放方法现在主要是内部使用，但也可以保留为public ---

    public void PlayBGM(AudioClip clip)
    {
        StopBGM();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
        Debug.Log(clip);
    }

    public void PlaySequencedBGM(SequencedBgm bgmSequence)
    {
        if (bgmSequence == null || bgmSequence.themeClip == null) return;
        StopBGM();
        bgmCoroutine = StartCoroutine(BgmSequenceCoroutine(bgmSequence));
        Debug.Log(bgmSequence);
    }

    public void StopBGM()
    {
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }
        bgmSource.Stop();
    }

    private IEnumerator BgmSequenceCoroutine(SequencedBgm bgm)
    {
        if (bgm.introClip != null)
        {
            bgmSource.clip = bgm.introClip;
            bgmSource.loop = false;
            bgmSource.Play();
            yield return new WaitWhile(() => bgmSource.isPlaying);
        }

        while (true)
        {
            bgmSource.clip = bgm.themeClip;
            bgmSource.loop = false;
            bgmSource.Play();
            yield return new WaitWhile(() => bgmSource.isPlaying);

            if (bgm.bridgeClip != null)
            {
                bgmSource.clip = bgm.bridgeClip;
                bgmSource.loop = false;
                bgmSource.Play();
                yield return new WaitWhile(() => bgmSource.isPlaying);
            }
        }
    }

    // --- FX 和 Vocal 方法保持不变 ---
    public void PlayFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = fxSource.outputAudioMixerGroup;
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.Play();
        Destroy(src, clip.length / pitch + 0.1f);
    }

    public void PlayVocal(AudioClip clip)
    {
        vocalSource.Stop();
        vocalSource.clip = clip;
        vocalSource.loop = false;
        vocalSource.Play();
    }
}