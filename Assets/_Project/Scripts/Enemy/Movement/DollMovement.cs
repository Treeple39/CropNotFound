using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollMovement : BaseMovement
{
    [Header("移动设置")]
    [SerializeField] private float dollMoveDuration = 3f;     // 初始移动持续时间
    [SerializeField] private float dollSpeed = 3f;        // 娃娃移动速度
    [SerializeField] private float waitTimeBetweenMoves = 1f; // 两次移动间的等待时间

    [Header("碰撞反馈")]
    [SerializeField] private float stunDuration = 0.2f;     // 被玩家碰撞后的眩晕持续时间

    private Vector3 dollMoveDirection;                        // 当前移动方向
    private float dollMoveTimer = 0f;                         // 移动计时器
    private bool isFirstMove = true;                      // 是否是初始移动
    private bool isMovingForward = true;                  // 是否正在向前移动

    private float waitTimer = 0f;                         // 等待计时器
    private bool isWaiting = false;                       // 是否在等待中
    private bool isStunned = false;                       // 是否处于眩晕状态
    private float stunTimer = 0f;                         // 眩晕计时器

    public Enemy enemy;

    public int BigCoinCount = 2;

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

        moveSpeed = dollSpeed;
        StartCoroutine(DelayedInit());
        StartNewMovementCycle(); // 改为调用新的启动方法
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        if (enemy.idleState == null || enemy.fleeState == null)
        {
            Debug.LogError("Enemy状态未初始化!");
            yield break;
        }
        enemy.stateMachine.ChangeState(enemy.fleeState);
    }

    protected override void Update()
    {
        base.Update();

        // 如果处于眩晕状态，则只处理眩晕计时
        if (isStunned)
        {
            HandleStun();
            return;
        }

        // 如果处于等待状态，则只处理等待计时
        if (isWaiting)
        {
            HandleWaiting();
            return;
        }

        // 如果在移动，则处理移动计时
        HandleMovement();
    }

    private void HandleStun()
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            isStunned = false;
            StartNewMovementCycle(); // 从一个新的完整周期开始
        }
    }

    private void HandleWaiting()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTimeBetweenMoves)
        {
            isWaiting = false;
            enemy.stateMachine.ChangeState(enemy.fleeState);
            MoveInCurrentDirection();
        }
    }

    private void HandleMovement()
    {
        dollMoveTimer += Time.deltaTime;
        float currentMoveDuration = isFirstMove ? dollMoveDuration : 2 * dollMoveDuration;

        if (dollMoveTimer >= currentMoveDuration)
        {
            StopAndPrepareForNextMove();
        }
    }

    private void StopAndPrepareForNextMove()
    {
        StopMove();

        // 切换移动方向
        if (isFirstMove)
        {
            isFirstMove = false;
            isMovingForward = false;
        }
        else
        {
            isMovingForward = !isMovingForward;
        }

        dollMoveTimer = 0f;
        isWaiting = true;
        waitTimer = 0f;
        enemy.stateMachine.ChangeState(enemy.idleState);
    }

    // 开始一个新的完整移动周期
    private void StartNewMovementCycle()
    {
        // 随机化一个全新的方向
        dollMoveDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0
        ).normalized;

        dollMoveTimer = 0f;
        isFirstMove = true;
        isMovingForward = true;
        isWaiting = false; // 确保不在等待状态
        isStunned = false; // 确保不在眩晕状态

        enemy.stateMachine.ChangeState(enemy.fleeState);
        MoveInCurrentDirection();
    }

    // 按当前方向移动
    private void MoveInCurrentDirection()
    {
        if (!gameObject.activeSelf || isStunned) return;
        Move(isMovingForward ? dollMoveDirection : -dollMoveDirection);
    }

    // ★★★【核心修改】★★★
    // 对碰撞处理进行精细化改造
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // 如果当前已经处于眩晕或等待状态，则不处理新的碰撞，避免逻辑混乱
        if (isStunned || isWaiting)
        {
            return;
        }

        // 1. 检查碰撞到的是否是玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            // --- 如果是玩家，执行眩晕逻辑 ---
            Debug.Log("娃娃撞到了玩家，进入眩晕状态！");
            StopMove();
            isStunned = true;
            stunTimer = stunDuration;
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
        else
        {
            // --- 如果是其他物体（墙壁、障碍物等），执行反向移动逻辑 ---
            Debug.Log("娃娃撞到了障碍物，立即反向移动！");

            // 立即反转方向
            isMovingForward = !isMovingForward;

            // 重置移动计时器，让它以新的方向完整地移动一段时间
            dollMoveTimer = 0f;

            // 立即以新的方向移动
            MoveInCurrentDirection();
        }
    }
}