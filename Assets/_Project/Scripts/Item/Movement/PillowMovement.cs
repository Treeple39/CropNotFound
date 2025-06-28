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

    private Vector3 lastMoveDirection;

    // 眩晕状态的计时器
    private float stunTimer = 0f;
    // 恐慌状态的计时器
    private float panicTimer = 0f;

    protected override void Start()
    {
        base.Start();
        
        // 设置初始缓慢速度
        moveSpeed = slowSpeed;
        
        // 选择一个随机方向开始移动
        ChooseRandomDirection();
        
        // 尝试找到玩家对象
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        
        // 记录当前移动方向用于反射计算
        lastMoveDirection = moveDirection;
        
        // 调用基类Update执行移动
        base.Update();
    }

    private void ChooseRandomDirection()
    {
        if (isStunned || isPanicking) return;
        
        // 生成一个随机方向
        float randomAngle = Random.Range(0f, 360f);
        Vector3 newDirection = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        );
        
        // 确保使用缓慢速度移动
        moveSpeed = slowSpeed;
        Move(newDirection);
    }

    private void CheckPlayerProximity()
    {
        if (playerTransform == null || isPanicking) return;
        
        // 计算与玩家的距离
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
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
        
        // 计算远离玩家的方向
        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        
        // 切换到高速状态
        moveSpeed = fastSpeed;
        Move(directionAwayFromPlayer);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        
        // 处理碰撞反射
        HandleReflection(collision);
    }
    
    private void HandleReflection(Collision collision)
    {
        // 获取碰撞点的法线
        ContactPoint contact = collision.contacts[0];
        Vector3 normal = contact.normal;
        
        // 计算反射方向: R = V - 2(V·N)N，其中V是入射向量，N是法线向量
        Vector3 reflectedDirection = Vector3.Reflect(lastMoveDirection, normal);
        reflectedDirection.y = 0; // 保持在水平面内移动
        
        // 应用反射方向（保持当前速度状态）
        Move(reflectedDirection);
    }
    
    // 可视化检测范围（仅在编辑器中可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
