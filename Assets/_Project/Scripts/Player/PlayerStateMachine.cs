using UnityEngine;

/// <summary>
/// 玩家状态枚举
/// </summary>
public enum PlayerState
{
    Idle,
    WalkUp,
    WalkDown,
    WalkLeft,
    WalkRight,
    Dizzy
}

/// <summary>
/// 通用玩家状态机：根据输入、移动速度及眩晕开关来切换 Spine 动画
/// 挂在玩家根节点，须同时存在：
///   - PlayerInputController
///   - PlayerMovementController
///   - SpineAnimationController
/// </summary>
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("速度低于此阈值视为静止")] public float moveThreshold = 0.1f;
    [Tooltip("是否强制进入眩晕状态")] public bool isDizzy;

    // 当前状态（Inspector 可看）
    [Header("Runtime State")] public PlayerState currentState;

    // 对应 Spine 动画名数组（下标顺序必须对应 PlayerState 枚举）
    private static readonly string[] animNames = {
        "wink",      // Idle
        "walk",   // WalkUp
        "walk", // WalkDown
        "run", // WalkLeft
        "run",// WalkRight
        "wink"      // Dizzy
    };

    // 依赖组件引用
    private PlayerInputController inputController;
    private PlayerMovementController movementController;
    private SpineAnimationController spineController;

    void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        movementController = GetComponent<PlayerMovementController>();
        spineController = GetComponentInChildren<SpineAnimationController>();

    }
    private void Start()
    {
        // 初始状态设为 Idle 并循环播放
        currentState = PlayerState.Idle;
        spineController.PlayAnimation(animNames[(int)currentState], true);
    }

    void Update()
    {
        // 计算下一帧应处于的状态
        PlayerState newState = DetermineState();
        if (newState != currentState)
        {
            currentState = newState;
            bool loop = currentState != PlayerState.Dizzy;
            spineController.PlayAnimation(animNames[(int)currentState], loop);
        }
    }

    /// <summary>
    /// 根据眩晕标志与移动速度判断玩家状态
    /// </summary>
    private PlayerState DetermineState()
    {
        if (isDizzy)
            return PlayerState.Dizzy;

        Vector2 vel = movementController.GetVelocity();
        if (vel.sqrMagnitude < moveThreshold * moveThreshold)
            return PlayerState.Idle;

        // 区分水平/垂直移动方向
        if (Mathf.Abs(vel.x) > Mathf.Abs(vel.y))
            return vel.x > 0 ? PlayerState.WalkRight : PlayerState.WalkLeft;
        else
            return vel.y > 0 ? PlayerState.WalkUp : PlayerState.WalkDown;
    }
}
