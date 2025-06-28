using UnityEngine;
using System.Collections;

// 这个脚本负责根据玩家的移动速度和方向，切换Spine骨骼动画
public class PlayerStateMachine : MonoBehaviour
{
    // 定义玩家可能存在的动画状态
    public enum PlayerState
    {
        Idle,       // 静止
        WalkUp,     // 向上走
        WalkDown,   // 向下走
        WalkLR,     // 水平走 (左右)
        Dizzy       // 眩晕 (特殊状态)
    }

    [Header("Spine控制器引用")]
    [Tooltip("按顺序填入状态对应的Spine控制器：Idle, WalkLR, WalkUp, WalkDown, Dizzy")]
    public SpineAnimationController[] stateControllers;

    [Header("速度阈值")]
    [Tooltip("速度超过该值视为开始行走")]
    public float walkThreshold = 0.1f;
    [Tooltip("速度超过该值视为进入跑步动画")]
    public float runThreshold = 2.0f;

    [Header("特殊状态控制")]
    [Tooltip("勾选此项会强制进入眩晕状态")]
    public bool isDizzy = false;

    [Header("动画名配置")]
    public string idleAnim = "idle";
    public string walkAnim = "walk";
    public string runAnim = "run";
    public string walkUpAnim = "walk_up";   // 建议为上下行走使用不同动画名，如果相同则保持一致
    public string runUpAnim = "run_up";
    public string walkDownAnim = "walk_down";
    public string runDownAnim = "run_down";
    public string dizzyAnim = "dizzy";
    public string winkAnim = "wink"; // 用于叠加的眨眼动画

    // --- 内部变量 ---
    private PlayerState currentState;
    private FootstepPlayer foot; // 脚步声播放器引用 (可选)
    private Rigidbody2D rb;      // Rigidbody2D引用，用于获取速度

    void Awake()
    {
        // 获取必要的组件引用
        foot = GetComponentInChildren<FootstepPlayer>();
        rb = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D组件

        // 安全检查：如果缺少Rigidbody2D，则打印错误并禁用脚本，防止后续报错
        if (rb == null)
        {
            Debug.LogError($"[{name}] PlayerStateMachine 脚本需要一个 Rigidbody2D 组件来获取速度，但未能找到。脚本将被禁用。");
            this.enabled = false;
        }
    }

    void Start()
    {
        // 游戏开始时，默认进入静止状态
        SetState(PlayerState.Idle);
        // 启动一个协程，用于随机播放眨眼等叠加动画
        StartCoroutine(WinkRoutine());
    }

    void Update()
    {
        // 如果脚本已被禁用，则不执行任何操作
        if (!this.enabled) return;

        // 1. 最高优先级的状态检查：眩晕
        if (isDizzy)
        {
            // 如果当前不是眩晕状态，则切换到眩晕状态
            if (currentState != PlayerState.Dizzy)
            {
                SetState(PlayerState.Dizzy);
            }
            // 进入眩晕后，不再执行下面的逻辑
            return;
        }

        // 2. 根据速度判断当前应该处于何种状态
        Vector2 currentVelocity = rb.velocity; // 直接从Rigidbody2D获取当前速度
        float speed = currentVelocity.magnitude;

        // 如果速度足够快，尝试播放脚步声
        if (speed > walkThreshold)
        {
            if (foot != null)
            {
                foot.TryStep();
            }
        }

        // 决定下一个状态
        PlayerState nextState;
        if (speed >= walkThreshold)
        {
            // 通过比较X和Y方向速度的绝对值，来判断主导方向是水平还是垂直
            if (Mathf.Abs(currentVelocity.x) > Mathf.Abs(currentVelocity.y))
            {
                nextState = PlayerState.WalkLR; // 水平方向为主
            }
            else
            {
                nextState = currentVelocity.y > 0 ? PlayerState.WalkUp : PlayerState.WalkDown; // 垂直方向为主
            }
        }
        else
        {
            nextState = PlayerState.Idle; // 速度不够，视为静止
        }

        // 如果计算出的新状态与当前状态不同，则执行切换
        if (nextState != currentState)
        {
            SetState(nextState);
        }
    }

    /// <summary>
    /// 切换到指定的新动画状态
    /// </summary>
    /// <param name="next">要切换的目标状态</param>
    void SetState(PlayerState next)
    {
        // 1. 隐藏所有状态的Spine控制器，确保干净的切换
        foreach (var ctrl in stateControllers)
        {
            if (ctrl != null)
            {
                ctrl.gameObject.SetActive(false);
            }
        }

        // 2. 准备动画参数
        SpineAnimationController targetController = null;
        string animationName = idleAnim;
        bool shouldFlip = false;
        bool shouldLoop = true;

        float currentSpeed = rb.velocity.magnitude;

        // 3. 根据目标状态，选择对应的控制器和动画
        switch (next)
        {
            case PlayerState.Idle:
                targetController = stateControllers[0];
                animationName = idleAnim;
                break;

            case PlayerState.WalkLR:
                targetController = stateControllers[1];
                shouldFlip = rb.velocity.x < 0; // 如果向左移动(x速度为负)，则翻转
                animationName = (currentSpeed >= runThreshold) ? runAnim : walkAnim; // 根据速度选择走或跑
                break;

            case PlayerState.WalkUp:
                targetController = stateControllers[2];
                animationName = (currentSpeed >= runThreshold) ? runUpAnim : walkUpAnim;
                break;

            case PlayerState.WalkDown:
                targetController = stateControllers[3];
                animationName = (currentSpeed >= runThreshold) ? runDownAnim : walkDownAnim;
                break;

            case PlayerState.Dizzy:
                targetController = stateControllers[4];
                animationName = dizzyAnim;
                shouldLoop = false; // 眩晕动画通常只播放一次
                break;
        }

        // 4. 安全检查并播放动画
        if (targetController == null)
        {
            Debug.LogError($"[{name}] 状态 {next} 没有在 stateControllers 数组中配置对应的Spine控制器！");
            return;
        }

        targetController.gameObject.SetActive(true); // 激活选定的控制器
        targetController.SetFlipX(shouldFlip);       // 设置水平翻转
        targetController.Play(animationName, shouldLoop); // 播放动画

        // 5. 更新当前状态
        currentState = next;
    }

    /// <summary>
    /// 一个无限循环的协程，用于随机播放叠加动画（如眨眼）
    /// </summary>
    private IEnumerator WinkRoutine()
    {
        while (true)
        {
            // 等待一个随机时间
            yield return new WaitForSeconds(Random.Range(3f, 8f));

            // 在特定状态下才播放眨眼动画，避免在眩晕或向上走时出现
            if (currentState == PlayerState.Idle || currentState == PlayerState.WalkLR || currentState == PlayerState.WalkDown)
            {
                // 确保Idle控制器存在
                if (stateControllers.Length > 0 && stateControllers[0] != null)
                {
                    // 在轨道1上播放wink动画，这样它会叠加在主动画（轨道0）之上
                    stateControllers[0].Queue(winkAnim, false, null, trackIndex: 1);
                }
            }
        }
    }
}