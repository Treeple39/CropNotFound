using Spine;
using Spine.Unity;
using UnityEngine;
using System;

using SpineAnimState = Spine.AnimationState;

[RequireComponent(typeof(SkeletonAnimation))]
public class SpineAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private SpineAnimState state;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        // SkeletonAnimation.AnimationState 返回的也是 Spine.AnimationState
        state = skeletonAnimation.AnimationState; 
    }

    /// <summary>
    /// 立即播放动画，非循环时动画结束会触发 onComplete。
    /// 相同动画不会重复 SetAnimation。
    /// </summary>
    public void PlayAnimation(string animationName, bool loop = false, Action onComplete = null, int trackIndex = 0)
    {
        if (state.GetCurrent(trackIndex)?.Animation.Name == animationName)
            return;

        var entry = state.SetAnimation(trackIndex, animationName, loop);
        if (!loop && onComplete != null)
        {
            entry.Complete += (e) => {
                entry.Complete -= (ex) => { };
                onComplete();
            };
        }
    }
}