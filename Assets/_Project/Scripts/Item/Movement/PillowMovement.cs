using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowMovement : BaseMovement
{
    [Header("移动速度设置")]
    [SerializeField] private float slowSpeed = 2.5f; // 缓慢状态下的速度
    [SerializeField] private float fastSpeed = 10f; // 高速状态下的速度

    [Header("玩家检测设置")]
    [SerializeField] private float playerDetectionRadius = 5f;
    [SerializeField] private float panicDuration = 2f;
    [SerializeField] private float stunDuration = 3f;
    private bool isPanicking = false;
    private bool isStunned = false;
    private Transform playerTransform;

    // 将lastMoveDirection改为Vector2，更符合Rigidbody2D操作
    private Vector2 lastMoveDirection;

    // 眩晕状态的计时器
    private float stunTimer = 0f;
    // 恐慌状态的计时器
    private float panicTimer = 0f;

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
        // 设置初始缓慢速度
        moveSpeed = slowSpeed;
        
        // 选择一个随机方向开始移动
        ChooseRandomDirection();
        StartCoroutine(DelayedInit());
        // 尝试找到玩家对象
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        enemy.stateMachine.ChangeState(enemy.idleState);
    }
    protected override void Update()
    {
        if (isStunned)
        {
            // 眩晕状态下不移动，只计时
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                // 眩晕结束后选择新方向，使用缓慢速度
                moveSpeed = slowSpeed;
                ChooseRandomDirection();
            }
            return;
        }
        
        if (isPanicking)
        {
            // 恐慌状态计时
            panicTimer -= Time.deltaTime;
            if (panicTimer <= 0)
            {
                isPanicking = false;
                isStunned = true;
                stunTimer = stunDuration;
                StopMove();
            }
        }
        else
        {
            // 检测玩家距离
            CheckPlayerProximity();
        }
        
        // 记录当前移动方向用于反射计算，转换为Vector2格式
        if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = rb.velocity.normalized;
        }
        else
        {
            // 如果速度太小，则使用当前moveDirection
            lastMoveDirection = new Vector2(moveDirection.x, moveDirection.y).normalized;
        }
        
        // 调用基类Update执行移动
        base.Update();
    }

    private void ChooseRandomDirection()
    {
        if (isStunned || isPanicking) return;
        
        // 生成一个随机方向 (2D空间中的随机角度)
        float randomAngle = Random.Range(0f, 360f);
        Vector2 newDirection = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ).normalized;
        
        // 使用Vector3格式传递给Move方法
        Vector3 moveDir = new Vector3(newDirection.x, newDirection.y, 0);
        
        // 确保使用缓慢速度移动
        moveSpeed = slowSpeed;
        Move(moveDir);
    }

    private void CheckPlayerProximity()
    {
        if (playerTransform == null || isPanicking) return;
        
        // 计算与玩家的距离
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // 如果玩家在检测范围内，进入恐慌模式
        if (distanceToPlayer <= playerDetectionRadius)
        {
            EnterPanicMode();
        }
    }

    private void EnterPanicMode()
    {
        isPanicking = true;
        panicTimer = panicDuration;
        
        // 计算远离玩家的方向 (2D)
        Vector2 directionAwayFromPlayer = ((Vector2)(transform.position - playerTransform.position)).normalized;
        
        // 转换为Vector3格式
        Vector3 moveDir = new Vector3(directionAwayFromPlayer.x, directionAwayFromPlayer.y, 0);
        
        // 切换到高速状态
        moveSpeed = fastSpeed;
        Move(moveDir);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        
        // 处理碰撞反射
        HandleReflection(collision);
    }
    
    // 添加OnCollisionStay2D来处理持续碰撞情况
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        // 如果物体被卡住或粘连，尝试调整方向
        if (rb != null && rb.velocity.magnitude < 0.2f && !isStunned)
        {
            // 获取当前碰撞点的法线
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;
            
            // 尝试沿着法线稍微偏移一下方向，避免粘连
            Vector2 adjustedDirection = lastMoveDirection + normal * 0.5f;
            Move(new Vector3(adjustedDirection.x, adjustedDirection.y, 0));
        }
    }
    
    private void HandleReflection(Collision2D collision)
    {
        // 获取碰撞点的法线
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;
        
        // 计算反射方向: R = V - 2(V·N)N，其中V是入射向量，N是法线向量
        Vector2 reflectedDirection = Vector2.Reflect(lastMoveDirection, normal);
        
        // 应用反射方向（保持当前速度状态）
        Move(new Vector3(reflectedDirection.x, reflectedDirection.y, 0));
    }
    
    // 可视化检测范围（仅在编辑器中可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
