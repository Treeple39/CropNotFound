using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMovement : BaseMovement
{
    [Header("玩家检测")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Transform player;

    [Header("椅子行为设置")]
    [Tooltip("正常状态下的缓慢持续移动速度")]
    [SerializeField] private float wanderSpeed = 0.5f;

    [Tooltip("玩家靠近后，冲刺的速度")]
    [SerializeField] private float dashSpeed = 8f;

    [Tooltip("每次冲刺持续的时间（秒）")]
    [SerializeField] private float dashDuration = 0.25f;

    [Tooltip("玩家靠近后，两次冲刺之间的间隔时间（秒）")]
    [SerializeField] private float dashInterval = 1.5f;

    // --- 核心状态标志位和计时器 ---
    private bool isRunningAway = false;
    private float dashCooldownTimer = 0f;

    // --- 用于持续缓慢移动的变量 ---
    private Vector2 wanderDirection;
    private float wanderDirectionChangeTimer;
    [Tooltip("缓慢移动时，改变方向的频率（秒）")]
    [SerializeField] private float wanderDirectionChangeInterval = 3f;


    #region 状态机
    public Enemy enemy;
    private bool isInitialized = false;
    #endregion

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("缺少Enemy组件!", gameObject);
            enabled = false;
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // 初始化游荡方向
        PickNewWanderDirection();
        wanderDirectionChangeTimer = wanderDirectionChangeInterval;

        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitUntil(() => enemy.idleState != null && enemy.fleeState != null);
        isInitialized = true;
        enemy.stateMachine.Initialize(enemy.idleState);
        isRunningAway = false;
    }

    protected override void Update()
    {
        base.Update();
        if (!isInitialized) return;

        // --- 1. 状态决策 ---
        bool isPlayerNearby = (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius);

        if (isPlayerNearby && !isRunningAway)
        {
            StartFleeing();
        }
        else if (!isPlayerNearby && isRunningAway)
        {
            StopFleeing();
        }

        // --- 2. 行为执行 ---
        // 关键：如果正在冲刺中，则让冲刺完成，不执行其他移动
        if (IsMoving)
        {
            // 注意：IsMoving是由BaseMovement的Move方法设置的，在dashDuration后会自动变回false
            return;
        }

        if (isRunningAway)
        {
            // 处于逃跑状态时，执行间歇性冲刺
            HandleDashing();
        }
        else
        {
            // 处于空闲状态时，执行持续缓慢游荡
            HandleWandering();
        }
    }

    private void StartFleeing()
    {
        isRunningAway = true;
        enemy.stateMachine.ChangeState(enemy.fleeState);
        dashCooldownTimer = 0;
        StopMove(); // 立即停止当前的缓慢移动，准备冲刺
        Debug.Log("椅子发现玩家，进入受惊状态！");
    }

    private void StopFleeing()
    {
        isRunningAway = false;
        enemy.stateMachine.ChangeState(enemy.idleState);
        StopMove(); // 确保停止所有移动
        PickNewWanderDirection(); // 为接下来的缓慢移动选择一个新方向
        Debug.Log("椅子恢复平静。");
    }

    /// <summary>
    /// 执行间歇性冲刺的逻辑 (此部分逻辑已正确)
    /// </summary>
    private void HandleDashing()
    {
        dashCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer <= 0)
        {
            moveSpeed = dashSpeed;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            // Move方法会设置IsMoving = true，并在dashDuration后结束
            Move(randomDirection, dashDuration);
            dashCooldownTimer = dashInterval;
        }
    }

    /// <summary>
    /// 执行持续缓慢游荡的逻辑 (已修改为持续移动)
    /// </summary>
    private void HandleWandering()
    {
        // 更新方向改变计时器
        wanderDirectionChangeTimer -= Time.deltaTime;
        if (wanderDirectionChangeTimer <= 0)
        {
            PickNewWanderDirection();
        }

        // 设置移动速度
        moveSpeed = wanderSpeed;
        // 假设BaseMovement的rb是public或protected的。
        if (rb != null) // rb 是 BaseMovement 中的 Rigidbody2D
        {
            rb.velocity = wanderDirection * moveSpeed;
        }
    }

    /// <summary>
    /// 为持续缓慢移动选择一个新的随机方向
    /// </summary>
    private void PickNewWanderDirection()
    {
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderDirectionChangeTimer = wanderDirectionChangeInterval;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}