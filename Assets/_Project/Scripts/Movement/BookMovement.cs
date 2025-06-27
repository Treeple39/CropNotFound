using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMovement : BaseMovement
{
    [SerializeField] private float minMoveTime = 0.5f;        // 最小移动时间
    [SerializeField] private float maxMoveTime = 2.0f;        // 最大移动时间
    [SerializeField] private float bookSpeed = 3.0f;          // 书本移动速度
    [SerializeField] private float playerDetectionRadius = 3f; // 检测玩家的半径
    [SerializeField] private Transform player;                // 玩家对象引用

    private float currentMoveTime;                            // 当前移动时间
    private float moveTimer;                                  // 移动计时器
    private Vector3 randomDirection;                          // 随机移动方向
    private bool isFleeingFromPlayer = false;                 // 是否正在从玩家处逃离

    protected override void Start()
    {
        base.Start();
        
        // 设置书本的移动速度
        moveSpeed = bookSpeed;
        
        // 如果未指定玩家，尝试在场景中查找玩家对象
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogWarning("未找到玩家对象，书本将无法检测玩家");
            }
        }
        
        // 开始第一次随机移动
        ChooseNewRandomDirection();
    }

    protected override void Update()
    {
        base.Update();
        
        // 检查是否有玩家在附近
        CheckForPlayer();
        
        // 更新移动计时器
        moveTimer += Time.deltaTime;
        
        // 如果当前移动时间已到，选择新的随机方向
        if (moveTimer >= currentMoveTime && !isFleeingFromPlayer)
        {
            ChooseNewRandomDirection();
        }
    }
    
    // 检查玩家是否在附近
    private void CheckForPlayer()
    {
        if (player != null)
        {
            // 计算与玩家的距离
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            // 如果玩家在检测范围内
            if (distanceToPlayer <= playerDetectionRadius)
            {
                // 如果还没有开始逃离玩家
                if (!isFleeingFromPlayer)
                {
                    // 开始逃离玩家
                    FleeFromPlayer();
                }
            }
            else if (isFleeingFromPlayer)
            {
                // 玩家不在范围内了，恢复随机移动
                isFleeingFromPlayer = false;
                ChooseNewRandomDirection();
            }
        }
    }
    
    // 选择新的随机方向
    private void ChooseNewRandomDirection()
    {
        // 重置移动计时器
        moveTimer = 0f;
        
        // 随机生成新的移动时间
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0 
        ).normalized;
        
        // 使用单参数Move函数持续移动
        Move(randomDirection);
        
        Debug.Log("书本选择了新的随机方向");
    }
    
    // 从玩家处逃离
    private void FleeFromPlayer()
    {
        isFleeingFromPlayer = true;
        
        // 计算远离玩家的方向
        Vector3 awayFromPlayerDirection = (transform.position - player.position).normalized;
        
        // 使用单参数Move函数持续移动
        Move(awayFromPlayerDirection);
        
        Debug.Log("书本发现了玩家，开始逃离！");
    }
    
    // 在编辑器中显示检测范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
