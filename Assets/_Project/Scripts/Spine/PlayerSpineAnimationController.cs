using UnityEngine;
using System;

/// <summary>
/// 专门用于玩家角色的 Spine 动画控制器，封装常用动画状态切换。
/// 不修改通用 SpineAnimationController，单独建立此脚本供 Player 使用。
/// 需挂载在玩家根节点，并在 Inspector 中关联 SpineAnimationController。
/// </summary>
[DisallowMultipleComponent]
public class PlayerSpineAnimationController : MonoBehaviour
{
    [Header("引用：通用 Spine 控制器")]
    [Tooltip("拖拽场景中挂有 SpineAnimationController 的子物体或者本物体")]  
    public SpineAnimationController spineController;

    [Header("动画名称（必须与 Spine 项目中一致）")]
    public string idleAnim      = "idle";
    public string walkAnim      = "walk";
    public string walkUpAnim    = "walk_up";
    public string walkDownAnim  = "walk_down";
    public string dizzyAnim     = "dizzy";
    public string jumpStartAnim = "jump_start";
    public string jumpAirAnim   = "jump_air";

    void Reset()
    {
        // 自动寻找，如果忘了拖
        if (spineController == null)
            spineController = GetComponentInChildren<SpineAnimationController>();
    }

    void Awake()
    {
        if (spineController == null)
        {
            Debug.LogError("PlayerSpineAnimationController: 缺少 SpineAnimationController 引用", this);
            enabled = false;
        }
    }

    /// <summary>
    /// 播放 Idle 动画
    /// </summary>
    public void PlayIdle()
    {
        spineController.SetFlipX(false);
        spineController.PlayAnimation(idleAnim, true);
    }

    /// <summary>
    /// 播放行走动画，参数 isFacingRight 决定水平镜像
    /// </summary>
    public void PlayWalk(bool isFacingRight)
    {
        spineController.SetFlipX(!isFacingRight);
        spineController.PlayAnimation(walkAnim, true);
    }

    /// <summary>
    /// 播放向上/向下行走
    /// </summary>
    public void PlayWalkUp()    { spineController.SetFlipX(false); spineController.PlayAnimation(walkUpAnim, true); }
    public void PlayWalkDown()  { spineController.SetFlipX(false); spineController.PlayAnimation(walkDownAnim, true); }

    /// <summary>
    /// 播放眩晕动画
    /// </summary>
    public void PlayDizzy()
    {
        spineController.SetFlipX(false);
        spineController.PlayAnimation(dizzyAnim, false);
    }

    /// <summary>
    /// 播放跳跃开始与空中动画
    /// </summary>
    public void PlayJump(Action onComplete = null)
    {
        // Jump_Start 接着 Jump_Air
        spineController.PlayAnimation(jumpStartAnim, false, () =>
        {
            spineController.PlayAnimation(jumpAirAnim, false, onComplete);
        });
    }
}
