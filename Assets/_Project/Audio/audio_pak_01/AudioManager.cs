using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager S;

    public AudioSource bgmSource, fxSource, vocalSource;
    AudioMixer mixer;

    void Awake()
    {
        S = this;
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
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = fxSource.outputAudioMixerGroup;
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.Play();
        // 根据 pitch 调整销毁时间，防止裁剪
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