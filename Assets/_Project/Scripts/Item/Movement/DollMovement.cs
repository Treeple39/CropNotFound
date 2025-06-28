using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DollMovement : BaseMovement
{
    [SerializeField] private float movementDistance = 3f; // 移动距离
    [SerializeField] private float dollSpeed = 3f; // 娃娃移动速度
    [SerializeField] private float waitTimeBetweenMoves = 1f; // 两次移动间的等待时间
    [SerializeField] private float stunDuration = 2f; // 眩晕持续时间

    public Vector3 targetPoint1; // 第一个目标点
    public Vector3 targetPoint2; // 第二个目标点
    private Vector3 currentTarget; // 当前目标点
    private bool movingToTarget1 = true; // 是否正在向目标点1移动
    
    private float waitTimer = 0f; // 等待计时器
    private bool isWaiting = false; // 是否在等待中
    private bool hasReachedTarget = false; // 是否已经到达目标点
    
    // 眩晕相关变量
    private bool isStunned = false; // 是否处于眩晕状态
    private float stunTimer = 0f; // 眩晕计时器

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

        // 设置娃娃的移动速度
        moveSpeed = dollSpeed;
        
        // 设置初始目标点
        ResetTargetPoints();
        
        StartCoroutine(DelayedInit());
        // 开始第一次移动
        MoveToCurrentTarget();
    }
    
    // 重置目标点
    private void ResetTargetPoints()
    {
        // 随机一个方向（在XY平面上）
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0 // 在XY平面上移动，Z轴为0
        ).normalized;
        
        // 设置两个目标点
        targetPoint1 = transform.position + randomDirection * movementDistance;
        targetPoint2 = transform.position - randomDirection * movementDistance;
        
        // 设置初始目标点
        currentTarget = targetPoint1;
        movingToTarget1 = true;
    }
    
    private IEnumerator DelayedInit()
    {
        yield return null;

        if (enemy.idleState == null || enemy.fleeState == null)
        {
            Debug.LogError("Enemy状态未初始化!");
            yield break;
        }
        isInitialized = true;
        enemy.stateMachine.ChangeState(enemy.fleeState);
    }

    protected override void Update()
    {
        base.Update();
        
        // 如果处于眩晕状态
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                // 眩晕结束
                isStunned = false;
                
                // 重置目标点
                ResetTargetPoints();
                
                // 开始等待
                isWaiting = true;
                waitTimer = 0f;
                
                if (isInitialized)
                {
                    enemy.stateMachine.ChangeState(enemy.idleState);
                }
            }
            return; // 眩晕状态下不执行其他逻辑
        }
        
        // 如果正在等待
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeBetweenMoves)
            {
                isWaiting = false;
                if (isInitialized)
                {
                    enemy.stateMachine.ChangeState(enemy.fleeState);
                }
                MoveToCurrentTarget();
            }
            return;
        }
        
        // 检查是否到达了目标点附近
        if (!hasReachedTarget && Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            // 已到达目标点
            hasReachedTarget = true;
            StopMove();

            if (isInitialized)
            {
                enemy.stateMachine.ChangeState(enemy.idleState);
            }

            SwitchTarget();
            isWaiting = true;
            waitTimer = 0f;
        }
    }
    
    // 切换目标点
    private void SwitchTarget()
    {
        movingToTarget1 = !movingToTarget1;
        currentTarget = movingToTarget1 ? targetPoint1 : targetPoint2;
    }
    
    // 向当前目标点移动
    private void MoveToCurrentTarget()
    {
        if (!gameObject.activeSelf || isStunned) return;

        hasReachedTarget = false;
        
        // 计算移动方向
        Vector3 direction = (currentTarget - transform.position).normalized;
        
        // 使用新的单参数Move方法实现连续移动
        Move(direction);
    }
    
    // 碰撞处理
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        
        // 碰撞后进入眩晕状态
        EnterStunnedState();
    }
    
    // 进入眩晕状态
    private void EnterStunnedState()
    {
        // 停止当前移动
        StopMove();
        
        // 设置眩晕状态
        isStunned = true;
        stunTimer = stunDuration;
        
        // 重置其他状态
        isWaiting = false;
        hasReachedTarget = false;
        
        if (isInitialized)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
}
