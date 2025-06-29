using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioVisable : MonoBehaviour
{
    public static AudioVisable S;

    private void Awake()

    {
        S = this;
    }

    AudioSource audioSource;
    private AudioManager audioManager;
    public float[] samples = new float[512];

    void Start()
    {
        audioManager=GetComponent<AudioManager>();
        audioSource = audioManager.bgmSource;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            GetSpectrumAudioSource();
        }
        
    }
    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples,0,FFTWindow.Blackman);
    }

}
