using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机播放一组“对话氛围”音效，比如咳嗽、叹气、环境音……
/// 随机间隔、随机音量和音高，并循环播放
/// </summary>
[DisallowMultipleComponent]
public class DialogueAmbiencePlayer : MonoBehaviour
{
    [Header("可选的氛围音列表 (随机挑一)")]
    public List<AudioClip> ambienceClips;

    [Header("基础间隔 (秒)")]
    public float baseInterval = 5f;

    [Header("间隔抖动范围 (±秒)")]
    public float intervalVariance = 1f;

    [Header("音量范围")]
    [Range(0f,1f)] public Vector2 volumeRange = new Vector2(0.3f, 0.6f);

    [Header("音高范围")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Header("是否自动在 Start 时启动")]
    public bool playOnStart = true;

    private Coroutine _playRoutine;

    void Start()
    {
        if (playOnStart && ambienceClips != null && ambienceClips.Count > 0)
            _playRoutine = StartCoroutine(AmbienceLoop());
    }

    /// <summary>
    /// 外部调用可手动开始/停止
    /// </summary>
    public void StartAmbience()
    {
        if (_playRoutine == null && ambienceClips != null && ambienceClips.Count > 0)
            _playRoutine = StartCoroutine(AmbienceLoop());
    }

    public void StopAmbience()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
    }

    private IEnumerator AmbienceLoop()
    {
        while (true)
        {
            // 随机下一次播放的间隔
            float iv = Random.Range(-intervalVariance, intervalVariance);
            float wait = Mathf.Max(0.1f, baseInterval + iv);
            yield return new WaitForSeconds(wait);

            // 随机挑一个音
            var clip = ambienceClips[Random.Range(0, ambienceClips.Count)];
            if (clip == null) 
                continue;

            // 随机音量和音高
            float vol = Random.Range(volumeRange.x, volumeRange.y);
            float pit = Random.Range(pitchRange.x, pitchRange.y);

            // 播放（假设你有 AudioManager 单例的 PlayFX）
            AudioManager.S.PlayFX(clip, Mathf.Clamp01(vol), pit);
        }
    }
}