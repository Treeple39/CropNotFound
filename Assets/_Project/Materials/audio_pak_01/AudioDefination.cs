using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    [Header("挂载对应的SO文件")]
    public PlayAudioEventSO playAudioEventSo;
    [Header("播放的音频")]
    public AudioClip audioClip;
    [Header("是否自动播放")]
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
            PlayAudioClip();
    }

    public void PlayAudioClip()
    {
        if (playAudioEventSo != null)
            playAudioEventSo.OnEventRaised_Invoke(audioClip);
    }

    public void PlayTargetAudioClip(AudioClip clip)
    {
        if (playAudioEventSo != null)
            playAudioEventSo.OnEventRaised_Invoke(clip);
    }
}