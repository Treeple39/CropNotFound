using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

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
    public AudioSource bgmSource;
    private AudioSource fxSource;
    private AudioSource vocalSource;
    private AudioMixer mixer;
    private Coroutine bgmCoroutine;

    void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
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

    AudioSource MakeSource(string groupName)
    {
        var src = gameObject.AddComponent<AudioSource>();
        var group = mixer.FindMatchingGroups(groupName);
        if (group.Length > 0) src.outputAudioMixerGroup = group[0];
        return src;
    }

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