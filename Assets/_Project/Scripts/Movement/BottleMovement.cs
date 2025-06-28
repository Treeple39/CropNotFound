using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleMovement : BaseMovement
{
    [SerializeField] private float minStayTime = 3f;      // 最小停留时间（秒）
    [SerializeField] private float maxStayTime = 7f;      // 最大停留时间（秒）
    [SerializeField] private float teleportRadius = 10f;  // 传送半径范围
    [SerializeField] private int maxTeleportAttempts = 10; // 最大传送尝试次数
    [SerializeField] private LayerMask obstacleLayer = 1; // 障碍物层，设为Default层（值为1，对应第0层）
    
    private float stayTimer;                              // 停留计时器
    private float currentStayTime;                        // 当前停留时间
    private Vector3 startPosition;                        // 初始位置
    private Collider bottleCollider;                      // 瓶子的碰撞器
    
    protected override void Start()
    {
        base.Start();
        
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
        
        // 设置初始停留时间
        SetNewStayTime();
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 更新停留计时器
        stayTimer += Time.deltaTime;
        
        // 如果停留时间结束，进行传送
        if (stayTimer >= currentStayTime)
        {
            TeleportToRandomPosition();
            SetNewStayTime();
        }
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
        while (true)
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
