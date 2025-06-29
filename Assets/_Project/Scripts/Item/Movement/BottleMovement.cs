using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleMovement : BaseMovement
{
    [SerializeField] private float minStayTime = 2f;      // 最小停留时间（秒）
    [SerializeField] private float maxStayTime = 8f;      // 最大停留时间（秒）
    [SerializeField] private float teleportRadius = 10f;  // 传送半径范围
    [SerializeField] private int maxTeleportAttempts = 10; // 最大传送尝试次数
    [SerializeField] private LayerMask obstacleLayer = 1; // 障碍物层，设为Default层（值为1，对应第0层）
    
    private float stayTimer;                              // 停留计时器
    private float currentStayTime;                        // 当前停留时间
    private Collider bottleCollider;                      // 瓶子的碰撞器
    private bool isTransporting = false; // 添加标志变量，表示是否正在传送过程中

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

        // 保存初始位置
        startPosition = transform.position;
        
        // 获取碰撞器
        bottleCollider = GetComponent<Collider>();
        if (bottleCollider == null)
        {
            Debug.LogWarning("瓶子没有碰撞器组件，无法检测重叠");
        }
        
        // 确保障碍物层包含Default层
        obstacleLayer = LayerMask.GetMask("Default");
        Debug.Log($"瓶子将避开Default层的所有物体，LayerMask值: {obstacleLayer}");
        StartCoroutine(DelayedInit());
        // 设置初始停留时间
        SetNewStayTime();
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
        base.Update();

        StopMove();
        
        // 如果正在传送过程中，不进行计时
        if (isTransporting)
            return;
            
        // 更新停留计时器
        stayTimer += Time.deltaTime;
         
        // 如果停留时间结束，进行传送
        if (stayTimer >= currentStayTime)
        {
            isTransporting = true; // 设置标志，防止重复触发
            enemy.stateMachine.ChangeState(enemy.shineState);
            StartCoroutine(ChangeStateWithDelay());
        }
    }
    private IEnumerator ChangeStateWithDelay()
    {
        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);

        // 切换状态
        enemy.stateMachine.ChangeState(enemy.idleState);
        TeleportToRandomPosition();
        SetNewStayTime();
        isTransporting = false; // 传送完成，重置标志
    }
    // 设置新的停留时间
    private void SetNewStayTime()
    {
        stayTimer = 0f;
        currentStayTime = Random.Range(minStayTime, maxStayTime);
        Debug.Log($"瓶子将停留 {currentStayTime:F1} 秒");
    }
    
    // 传送到随机位置
    private void TeleportToRandomPosition()
    {
        // 持续尝试找到一个没有重叠的位置
        int attempts = 0;
        while (attempts < maxTeleportAttempts)
        {
            attempts++;
            
            // 生成一个在圆形区域内的随机点（在XY平面上）
            Vector2 randomOffset = Random.insideUnitCircle * teleportRadius;
            Vector3 randomPosition = startPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // 检查新位置是否有重叠
            if (!IsOverlapping(randomPosition))
            {
                // 找到有效位置，进行传送
                transform.position = randomPosition;
                Debug.Log($"瓶子在尝试{attempts}次后传送到了新位置: {randomPosition}");
                return;
            }
        }
        
        // 如果达到最大尝试次数仍未找到合适位置，保持在原位
        Debug.LogWarning($"瓶子尝试传送{maxTeleportAttempts}次后未找到合适位置，保持原位");
    }
    
    // 检查指定位置是否与其他物体重叠
    private bool IsOverlapping(Vector3 position)
    {
        if (bottleCollider == null) return false;
        
        // 保存原始位置
        Vector3 originalPosition = transform.position;
        
        // 临时移动到新位置进行检查
        transform.position = position;
        
        // 检测是否与Default层的物体重叠
        Collider[] colliders = Physics.OverlapBox(
            bottleCollider.bounds.center,
            bottleCollider.bounds.extents,
            transform.rotation,
            obstacleLayer
        );
        
        // 恢复原位置
        transform.position = originalPosition;
        
        // 如果检测到碰撞器（排除自身），则认为有重叠
        foreach (var collider in colliders)
        {
            if (collider != bottleCollider)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // 在编辑器中显示传送范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);
    }
}
