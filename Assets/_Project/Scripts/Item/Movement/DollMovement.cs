using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DollMovement : BaseMovement
{
    [SerializeField] private float movementDistance = 3f; // 移动距离
    [SerializeField] private float dollSpeed = 3f; // 娃娃移动速度
    [SerializeField] private float waitTimeBetweenMoves = 1f; // 两次移动间的等待时间

    public Vector3 targetPoint1; // 第一个目标点
    public Vector3 targetPoint2; // 第二个目标点
    private Vector3 currentTarget; // 当前目标点
    private bool movingToTarget1 = true; // 是否正在向目标点1移动
    
    private float waitTimer = 0f; // 等待计时器
    private bool isWaiting = false; // 是否在等待中
    private bool hasReachedTarget = false; // 是否已经到达目标点

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
        StartCoroutine(DelayedInit());
        // 开始第一次移动
        MoveToCurrentTarget();
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
        
        // 如果正在等待
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeBetweenMoves)
            {
                isWaiting = false;
                enemy.stateMachine.ChangeState(enemy.fleeState);
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
            
            // 强制设置位置到精确的目标点，避免误差累积
            transform.position = currentTarget;

            enemy.stateMachine.ChangeState(enemy.idleState);

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
        
        if (!gameObject.activeSelf) return;

        hasReachedTarget = false;
        
        // 计算移动方向
        Vector3 direction = (currentTarget - transform.position).normalized;
        
        // 使用新的单参数Move方法实现连续移动
        Move(direction);
    }
}
