using System;
using Spine;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// 通用 Spine 动画控制器：封装播放、队列、停止和水平/垂直镜像功能。
/// 可挂在任何带有 SkeletonAnimation 的 GameObject 上。
/// </summary>
[DisallowMultipleComponent]
public class SpineAnimationController : MonoBehaviour
{
    [Tooltip("可选：手动指定 SkeletonAnimation，否则自动查找子物体")]
    public SkeletonAnimation skeletonAnimation;

    private Spine.AnimationState state;
    private Skeleton skeleton;
    private string currentTrack0AnimationName;

    void Reset()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    void Awake()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null)
        {
            Debug.LogError("SpineAnimationController: 找不到 SkeletonAnimation 组件", this);
            enabled = false;
            return;
        }
        state = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
    }

    /// <summary>
    /// 立即播放动画，防止重复设置；non-loop 完毕后触发回调。
    /// </summary>
    public void Play(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0)
    {
        if (state == null) return;
        // 防止重复播放同一动画
        var current = state.GetCurrent(trackIndex)?.Animation.Name;
        if (current == animationName) return;
        var entry = state.SetAnimation(trackIndex, animationName, loop);

        // 如果是在主轨道(轨道0)上播放，我们就记录下这个动画的名字
        if (trackIndex == 0)
        {
            currentTrack0AnimationName = animationName;
        }
        if (!loop && onComplete != null)
            entry.Complete += e => onComplete();
    }
    /// <summary>
    /// 获取主轨道(Track 0)上当前正在播放的动画名称。
    /// </summary>
    /// <returns>动画名称字符串</returns>
    public string GetCurrentAnimationName(int trackIndex = 0)
    {
        // 方案一：直接返回我们自己记录的变量
        if (trackIndex == 0)
        {
            return currentTrack0AnimationName;
        }

        // 方案二（更通用）：直接从Spine状态获取，以备将来查询其他轨道
        var trackEntry = state.GetCurrent(trackIndex);
        return trackEntry?.Animation.Name; // 使用空值传播操作符，如果trackEntry为null则返回null
    }
    /// <summary>
    /// 简写：同 Play
    /// </summary>
    public void PlayAnimation(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0)
    {
        Play(animationName, loop, onComplete, trackIndex);
    }

    /// <summary>
    /// 将动画加入队列，延迟播放，支持叠加（如 Wink）
    /// </summary>
    public void Queue(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0, float delay = 0f)
    {
        if (state == null) return;
        var entry = state.AddAnimation(trackIndex, animationName, loop, delay);
        if (onComplete != null)
            entry.Complete += e => onComplete();
    }

    /// <summary>
    /// 停止指定轨道上的所有动画
    /// </summary>
    public void Stop(int trackIndex = 0)
    {
        if (state == null) return;
        state.ClearTrack(trackIndex);
    }

    /// <summary>
    /// 水平镜像，通过设置负 ScaleX 实现
    /// </summary>
    public void SetFlipX(bool flip)
    {
        if (skeleton == null) return;
        float baseScale = Mathf.Abs(skeleton.ScaleX);
        skeleton.ScaleX = flip ? -baseScale : baseScale;
    }

    /// <summary>
    /// 垂直镜像，通过设置负 ScaleY 实现
    /// </summary>
    public void SetFlipY(bool flip)
    {
        if (skeleton == null) return;
        float baseScale = Mathf.Abs(skeleton.ScaleY);
        skeleton.ScaleY = flip ? -baseScale : baseScale;
    }
}
