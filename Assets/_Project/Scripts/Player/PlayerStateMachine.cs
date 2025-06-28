using UnityEngine;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState
    {
        Idle, WalkUp, WalkDown, WalkLR, Dizzy
    }

    [Header("Spine控制器引用")]
    [Tooltip("按顺序填入状态对应的Spine控制器：Idle, WalkLR, WalkUp, WalkDown, Dizzy")]
    public SpineAnimationController[] stateControllers;

    [Header("速度阈值")]
    public float walkThreshold = 0.1f;
    public float runThreshold = 2.0f;

    [Header("特殊状态控制")]
    public bool isDizzy = false;

    [Header("动画名配置")]
    public string idleAnim = "idle";
    public string walkAnim = "walk";
    public string runAnim = "run";
    public string walkUpAnim = "walk_up";
    public string runUpAnim = "run_up";
    public string walkDownAnim = "walk_down";
    public string runDownAnim = "run_down";
    public string dizzyAnim = "dizzy";
    public string winkAnim = "wink";

    private PlayerState currentState;
    private FootstepPlayer foot;
    private Rigidbody2D rb;

    // ★★★ 新增：用于存储当前激活的控制器，方便更新 ★★★
    private SpineAnimationController currentController;

    void Awake()
    {
        foot = GetComponentInChildren<FootstepPlayer>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[{name}] PlayerStateMachine 需要 Rigidbody2D 组件，但未能找到。");
            this.enabled = false;
        }
    }

    void Start()
    {
        SetState(PlayerState.Idle);
        StartCoroutine(WinkRoutine());
    }

    void Update()
    {
        if (!this.enabled) return;

        if (isDizzy)
        {
            if (currentState != PlayerState.Dizzy) SetState(PlayerState.Dizzy);
            return;
        }

        Vector2 currentVelocity = rb.velocity;
        float speed = currentVelocity.magnitude;

        if (speed > walkThreshold && foot != null)
        {
            foot.TryStep();
        }

        // 决定下一个状态
        PlayerState nextState;
        if (speed >= walkThreshold)
        {
            if (Mathf.Abs(currentVelocity.x) > Mathf.Abs(currentVelocity.y))
            {
                nextState = PlayerState.WalkLR;
            }
            else
            {
                nextState = currentVelocity.y > 0 ? PlayerState.WalkUp : PlayerState.WalkDown;
            }
        }
        else
        {
            nextState = PlayerState.Idle;
        }

        // 如果状态需要切换，则调用SetState
        if (nextState != currentState)
        {
            SetState(nextState);
        }

        // ★★★★★【核心修改】★★★★★
        // 在状态切换之后，我们每一帧都检查是否需要更新动画（比如跑步/走路切换）或翻转
        HandleAnimationAndFlip(currentVelocity, speed);
        // ★★★★★★★★★★★★★★★★★★★
    }

    /// <summary>
    /// ★★★ 新增方法：每帧处理动画和翻转的更新 ★★★
    /// </summary>
    private void HandleAnimationAndFlip(Vector2 velocity, float speed)
    {
        if (currentController == null) return;

        // 1. 处理水平翻转 (只在WalkLR状态下有效)
        if (currentState == PlayerState.WalkLR)
        {
            bool shouldFlip = velocity.x < 0;
            currentController.SetFlipX(shouldFlip);
        }

        // 2. 处理走/跑动画的切换
        // 这一步可以确保即使速度变化了但状态没变（比如从走路加速到跑步），动画也能正确更新
        string targetAnimName = "";
        switch (currentState)
        {
            case PlayerState.WalkLR:
                targetAnimName = (speed >= runThreshold) ? runAnim : walkAnim;
                break;
            case PlayerState.WalkUp:
                targetAnimName = (speed >= runThreshold) ? runUpAnim : walkUpAnim;
                break;
            case PlayerState.WalkDown:
                targetAnimName = (speed >= runThreshold) ? runDownAnim : walkDownAnim;
                break;
            default:
                // 其他状态（如Idle, Dizzy）的动画在SetState时已经固定，无需每帧更新
                return;
        }

        // 如果当前播放的动画不是我们期望的动画，就切换它
        if (currentController.GetCurrentAnimationName() != targetAnimName)
        {
            currentController.Play(targetAnimName, true);
        }
    }


    void SetState(PlayerState next)
    {
        // 1. 隐藏所有控制器
        foreach (var ctrl in stateControllers)
        {
            if (ctrl != null) ctrl.gameObject.SetActive(false);
        }

        // 重置当前控制器
        currentController = null;

        // 2. 根据状态选择新的控制器
        switch (next)
        {
            case PlayerState.Idle: currentController = stateControllers[0]; break;
            case PlayerState.WalkLR: currentController = stateControllers[1]; break;
            case PlayerState.WalkUp: currentController = stateControllers[2]; break;
            case PlayerState.WalkDown: currentController = stateControllers[3]; break;
            case PlayerState.Dizzy: currentController = stateControllers[4]; break;
        }

        // 3. 安全检查并激活
        if (currentController == null)
        {
            Debug.LogError($"[{name}] 状态 {next} 没有配置对应的Spine控制器！");
            return;
        }

        currentController.gameObject.SetActive(true);

        // 4. 【修改】只在进入新状态时播放初始动画，翻转和跑/走切换由HandleAnimationAndFlip处理
        string initialAnim = "";
        bool loop = true;
        switch (next)
        {
            case PlayerState.Idle: initialAnim = idleAnim; break;
            case PlayerState.WalkLR: initialAnim = walkAnim; break; // 总是从走路动画开始
            case PlayerState.WalkUp: initialAnim = walkUpAnim; break;
            case PlayerState.WalkDown: initialAnim = walkDownAnim; break;
            case PlayerState.Dizzy: initialAnim = dizzyAnim; loop = false; break;
        }
        currentController.Play(initialAnim, loop);

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