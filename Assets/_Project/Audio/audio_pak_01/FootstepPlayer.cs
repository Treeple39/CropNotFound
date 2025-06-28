
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    public List<AudioClip> footsteps;
    [Header("基础步频（秒）")]
    public float baseInterval = 0.4f;
    [Header("步频抖动范围（±秒）")]
    public float intervalVariance = 0.1f;

    float lastStep;
    float nextStepTime;

    public void TryStep()
    {
        if (Time.time < nextStepTime) return;
        // 随机一下下一次触发时间
        float iv = Random.Range(-intervalVariance, intervalVariance);
        nextStepTime = Time.time + Mathf.Max(0.05f, baseInterval + iv);

        // 随机挑一条脚步声
        var clip = footsteps[Random.Range(0, footsteps.Count)];

        // 随机音量和音高
        float vol = 0.6f + Random.Range(-0.2f, 0.2f);
        float pit = 1.0f + Random.Range(-0.1f, 0.1f);
        AudioManager.S.PlayFX(clip, Mathf.Clamp01(vol), pit);
    }
}