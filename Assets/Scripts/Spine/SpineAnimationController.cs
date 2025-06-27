using Spine;
using Spine.Unity;
using UnityEngine;
using System;

/// <summary>
/// 通用 Spine 动画控制器，提供播放动画并注册完成回调的能力。
/// 可挂载到任何包含 SkeletonAnimation 的 GameObject 上使用。
/// </summary>
[RequireComponent(typeof(SkeletonAnimation))]
public class SpineAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState state;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        state = skeletonAnimation.AnimationState;
    }

    /// <summary>
    /// 立即播放指定动画，并在播放完成时触发回调。
    /// 如果 loop 为 true，则不会触发完成回调。
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="loop">是否循环</param>
    /// <param name="onComplete">播放完毕回调</param>
    /// <param name="trackIndex">使用的 Track 索引</param>
    public void PlayAnimation(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0)
    {
        if (state.GetCurrent(trackIndex)?.Animation.Name == animationName)
            return; // 相同动画不重复播放

        var entry = state.SetAnimation(trackIndex, animationName, loop);
        if (!loop && onComplete != null)
        {
            entry.Complete += HandleComplete;

            void HandleComplete(TrackEntry e)
            {
                e.Complete -= HandleComplete;
                onComplete?.Invoke();
            }
        }
    }

    /// <summary>
    /// 在当前动画队列末尾加入一段动画，播放完前一段后开始本段。
    /// </summary>
    public void QueueAnimation(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0, float delay = 0f)
    {
        var entry = state.AddAnimation(trackIndex, animationName, loop, delay);
        if (!loop && onComplete != null)
        {
            entry.Complete += HandleComplete;

            void HandleComplete(TrackEntry e)
            {
                e.Complete -= HandleComplete;
                onComplete?.Invoke();
            }
        }
    }

    /// <summary>
    /// 清空指定轨道上的所有动画
    /// </summary>
    public void ClearTrack(int trackIndex = 0)
    {
        state.ClearTrack(trackIndex);
    }

    /// <summary>
    /// 清空所有轨道
    /// </summary>
    public void ClearAllTracks()
    {
        state.ClearTracks();
    }

    /// <summary>
    /// 获取当前播放的动画名
    /// </summary>
    public string GetCurrentAnimation(int trackIndex = 0)
    {
        return state.GetCurrent(trackIndex)?.Animation.Name;
    }
}