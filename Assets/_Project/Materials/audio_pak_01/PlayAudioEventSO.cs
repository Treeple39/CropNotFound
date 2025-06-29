using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Audio/Play Audio Event")]
public class PlayAudioEventSO : ScriptableObject
{
    public UnityEvent<AudioClip> OnEventRaised = new UnityEvent<AudioClip>();

    public void OnEventRaised_Invoke(AudioClip clip)
    {
        OnEventRaised.Invoke(clip);
    }
}