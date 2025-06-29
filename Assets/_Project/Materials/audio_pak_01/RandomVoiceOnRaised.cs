using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class RandomVoiceOnRaised : MonoBehaviour
{
    public List<AudioClip> randomClip;
    private AudioSource voiceOnPlay;
    public AudioMixerGroup voiceGroup;
    public bool playVoiceImmediate = false;

    private void Start()
    {
        voiceOnPlay = this.AddComponent<AudioSource>();
        voiceOnPlay.outputAudioMixerGroup = voiceGroup;
        PlayRandomClip();

        if (playVoiceImmediate)
        {
            PlayVoice();
        }
    }
    
    public void OnDead()
    {
        if (voiceOnPlay != null)
        {
            Destroy(voiceOnPlay);
            this.GetComponent<AudioDefination>().PlayAudioClip();
        }
    }
    private void PlayRandomClip()
    {
        if (randomClip.Count > 0)
        {
            int randomIndex = Random.Range(0, randomClip.Count);
            voiceOnPlay.clip = randomClip[randomIndex];
        }
        else
        {
            Debug.LogError("没有插入角色语音.");
        }
    }

    public void PlayVoice()
    {
        voiceOnPlay.Play();
    }
}
