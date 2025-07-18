using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleMovement : BaseMovement
{
    [Header("传送设置")]
    [SerializeField] private float minStayTime = 3f;      // 最小停留时间
    [SerializeField] private float maxStayTime = 8f;      // 最大停留时间
    [SerializeField] private float teleportRadius = 10f;  // 传送区域半径
    [SerializeField] private int maxTeleportAttempts = 10;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("原地移动设置")]
    [Tooltip("开始移动时的圆周半径")]
    [SerializeField] private float startCircleRadius = 2f;
    [Tooltip("传送前的最小圆周半径")]
    [SerializeField] private float endCircleRadius = 0.1f;
    [Tooltip("开始移动时的角速度 (控制旋转快慢)")]
    [SerializeField] private float startMoveSpeed = 5f;
    [Tooltip("传送前的最大角速度")]
    [SerializeField] private float endMoveSpeed = 20f;

    private float stayTimer;
    private float currentStayTime;
    private Collider bottleCollider;
    private bool isTransporting = false;

    // --- 新增变量 ---
    private Vector3 circleCenter; // 当前圆周运动的中心点
    private float currentAngle;   // 当前在圆周上的角度

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

        startPosition = transform.position;
        circleCenter = transform.position; // 初始圆心就是出生点

        bottleCollider = GetComponent<Collider>();
        obstacleLayer = LayerMask.GetMask("Default");

        StartCoroutine(DelayedInit());
        SetNewStayTime();
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        if (enemy.idleState == null || enemy.fleeState == null || enemy.shineState == null)
        {
            Debug.LogError("Enemy状态未初始化!");
            yield break;
        }
        isInitialized = true;
        enemy.stateMachine.ChangeState(enemy.idleState);
    }

    protected override void Update()
    {
        base.Update();
        // 如果正在传送的动画过程中，则不做任何事
        if (isTransporting)
            return;

        stayTimer += Time.deltaTime;

        // 在等待时间内，执行圆周运动
        HandleCircularMovement();

        // 如果停留时间结束，启动传送协程
        if (stayTimer >= currentStayTime)
        {
            isTransporting = true;
            enemy.stateMachine.ChangeState(enemy.shineState);
            StartCoroutine(TeleportSequence());
        }
    }

    /// <summary>
    /// 处理瓶子在等待传送时的圆周运动
    /// </summary>
    private void HandleCircularMovement()
    {
        if (currentStayTime <= 0) return; // 防止除以零

        // 1. 计算当前进度 (范围 0.0 to 1.0)
        float progress = Mathf.Clamp01(stayTimer / currentStayTime);

        // 2. 根据进度，使用Lerp(线性插值)计算当前的半径和速度
        float currentRadius = Mathf.Lerp(startCircleRadius, endCircleRadius, progress);
        float currentSpeed = Mathf.Lerp(startMoveSpeed, endMoveSpeed, progress);

        // 3. 更新角度，使其旋转起来
        currentAngle += currentSpeed * Time.deltaTime;

        // 4. 使用三角函数计算在圆周上的新位置
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle) * currentRadius, Mathf.Sin(currentAngle) * currentRadius, 0);

        // 5. 更新瓶子的实际位置
        transform.position = circleCenter + offset;
    }

    // 将传送逻辑整合到一个协程中
    private IEnumerator TeleportSequence()
    {
        // 等待闪光特效的动画/时间 (0.5秒)
        yield return new WaitForSeconds(0.5f);

        // 切换回空闲状态
        enemy.stateMachine.ChangeState(enemy.idleState);

        // 执行传送并设置新的圆心
        TeleportToRandomPosition();

        // 为下一次循环设置新的停留时间
        SetNewStayTime();

        // 传送完成，重置标志，允许下一次循环开始
        isTransporting = false;
    }

    private void SetNewStayTime()
    {
        stayTimer = 0f;
        currentStayTime = Random.Range(minStayTime, maxStayTime);
    }

    private void TeleportToRandomPosition()
    {
        int attempts = 0;
        while (attempts < maxTeleportAttempts)
        {
            attempts++;
            Vector2 randomOffset = Random.insideUnitCircle * teleportRadius;
            Vector3 randomPosition = startPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            // IsOverlapping 方法在这里不需要修改
            if (!IsOverlapping(randomPosition))
            {
                transform.position = randomPosition;
                // 关键：传送后，将新位置设置为下一次圆周运动的中心
                circleCenter = transform.position;
                return;
            }
        }
        Debug.LogWarning($"瓶子尝试传送{maxTeleportAttempts}次后未找到合适位置，保持原位");
        // 如果失败，圆心保持在原位
    }

    // IsOverlapping 方法无需修改
    private bool IsOverlapping(Vector3 position)
    {
        // ... (此方法保持原样)
        if (bottleCollider == null) return false;
        Collider[] colliders = Physics.OverlapBox(position, bottleCollider.bounds.extents, transform.rotation, obstacleLayer);
        foreach (var collider in colliders)
        {
            if (collider != bottleCollider) return true;
        }
        return false;
    }

    // OnDrawGizmosSelected 方法无需修改
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPosition, teleportRadius); // 显示总的传送区域
    }
}