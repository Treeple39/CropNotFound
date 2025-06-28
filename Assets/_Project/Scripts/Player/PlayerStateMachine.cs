using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        WalkUp,
        WalkDown,
        WalkLR,
        Dizzy
    }

    [Header("按顺序填：Idle, WalkLR, WalkUp, WalkDown, Dizzy")]
    public SpineAnimationController[] stateControllers;

    [Header("本地阈值")]
    public float walkThreshold = 0.1f;
    public float runThreshold = 2.0f;
    public bool isDizzy = false;

    [Header("动画名配置")]
    public string idleAnim = "idle";
    public string walkAnim = "walk";
    public string runAnim = "run";
    public string walkUpAnim = "walk";
    public string runUpAnim = "run";
    public string walkDownAnim = "walk";
    public string runDownAnim = "run";
    public string dizzyAnim = "dizzy";
    public string winkAnim = "wink";

    PlayerState currentState;

    FootstepPlayer foot;
    PlayerMovementController mov;

    void Awake()
    {
        foot = GetComponentInChildren<FootstepPlayer>();
        mov = GetComponent<PlayerMovementController>();
    }
    void Start()
    {
        SetState(PlayerState.Idle);
        // 启动 Wink 协程（可选）
        StartCoroutine(WinkRoutine());
    }

    void Update()
    {
        // 1) 优先 Dizzy
        if (isDizzy)
        {
            if (currentState != PlayerState.Dizzy)
                SetState(PlayerState.Dizzy);
            return;
        }

        // 2) 根据速度判断
        Vector2 vel = mov.GetVelocity();
        float speed = vel.magnitude;
        if (vel.magnitude > walkThreshold)
        {
            foot.TryStep();
        }
        // 区分上下 vs 左右
        PlayerState next;
        if (speed >= walkThreshold)
        {
            if (Mathf.Abs(vel.x) > Mathf.Abs(vel.y))
                next = PlayerState.WalkLR;
            else
                next = vel.y > 0 ? PlayerState.WalkUp : PlayerState.WalkDown;
        }
        else
        {
            next = PlayerState.Idle;
        }

        if (next != currentState)
            SetState(next);
    }

    void SetState(PlayerState next)
    {
        // 先关所有骨骼
        foreach (var ctrl in stateControllers)
            if (ctrl) ctrl.gameObject.SetActive(false);

        // 选控制器和动画名
        SpineAnimationController c = null;
        string anim = idleAnim;
        bool flip = false;
        bool loop = true;

        Vector2 vel = mov.GetVelocity();
        float speed = vel.magnitude;

        switch (next)
        {
            case PlayerState.Idle:
                c = stateControllers[0];
                anim = idleAnim;
                break;

            case PlayerState.WalkLR:
                c = stateControllers[1];
                flip = vel.x < 0; // x<0 向左
                anim = (speed >= runThreshold) ? runAnim : walkAnim;
                break;

            case PlayerState.WalkUp:
                c = stateControllers[2];
                anim = (speed >= runThreshold) ? runUpAnim : walkUpAnim;
                break;

            case PlayerState.WalkDown:
                c = stateControllers[3];
                anim = (speed >= runThreshold) ? runDownAnim : walkDownAnim;
                break;

            case PlayerState.Dizzy:
                c = stateControllers[4];
                anim = dizzyAnim;
                loop = false;
                break;
        }

        if (c == null)
        {
            Debug.LogError($"[{name}] 没给状态 {next} 绑定控制器");
            return;
        }

        c.gameObject.SetActive(true);
        c.SetFlipX(flip);
        c.Play(anim, loop);

        currentState = next;
    }

    // Wink 叠加示例
    System.Collections.IEnumerator WinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 8f));
            if (currentState != PlayerState.Dizzy && currentState != PlayerState.WalkUp)
            {
                // 用第二轨道让 wink 渲染在最上层
                stateControllers[(int)PlayerState.Idle]
                  .Queue(winkAnim, false, null, trackIndex: 1);
            }
        }
    }
}