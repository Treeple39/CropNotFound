using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlippersMovement : BaseMovement
{
    [Header("击退设置")]
    [SerializeField] private float knockbackForce = 12f;    // 击退力量
    [SerializeField] private float knockbackDuration = 0.2f; // 击退持续时间

    [Header("随机移动设置")]
    [SerializeField] private float wanderRadius = 4f;      // 随机移动半径
    [SerializeField] private float wanderInterval = 0.2f;  // 随机移动间隔
    private float wanderTimer;                            // 随机移动计时器
    private Vector3 wanderStartPosition;                  // 初始位置

    [Header("冲刺设置")]
    [SerializeField] private float chargeSpeed = 9f;     // 冲刺速度
    [SerializeField] private float normalSpeed = 2f;      // 普通移动速度
    [SerializeField] private float chargeCooldown = 3f;   // 冲刺冷却时间
    private bool isInCooldown = false;                   // 是否在冷却中
    private bool isCharging = false;                     // 是否正在冲刺

    [Header("玩家检测")]
    [SerializeField] private float detectRange = 3f;      // 检测范围
    [SerializeField] private LayerMask playerLayer;       // 玩家层级
    private Transform player;                            // 玩家Transform

    public Enemy enemy;

    public int BigCoinCount = 5;


    protected override void Start()
    {
        base.Start();
        moveSpeed = normalSpeed;
        
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("未找到Enemy组件!", gameObject);
            enabled = false;
            return;
        }
        
        // 保存初始位置
        wanderStartPosition = transform.position;
        
        // 查找玩家
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("未找到玩家对象!", gameObject);
        }
        
        // 如果没有设置玩家层级，则设置默认值
        if (playerLayer == 0)
        {
            playerLayer = LayerMask.GetMask("Player");
        }
        
        // 设置初始计时器
        wanderTimer = wanderInterval;
        
        // 延迟初始化状态机
        StartCoroutine(DelayedInit());
    }
    
    private IEnumerator DelayedInit()
    {
        yield return null;
        if (enemy.idleState != null && enemy.dashState != null)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
        else
        {
            Debug.LogError("Enemy状态未初始化!");
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (!canMove) return;
        
        // 确保有玩家对象
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        // 如果不在冷却中且未在冲刺，检测玩家
        if (!isInCooldown && !isCharging && DetectPlayer())
        {
            StartCharge();
        }
        else if (!isCharging)
        {
            // 执行随机移动
            Wander();
        }
    }
    
    // 检测玩家
    private bool DetectPlayer()
    {
        if (player == null) return false;
        
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= detectRange;
    }
    
    // 开始冲刺
    private void StartCharge()
    {
        if (player == null) return;
        
        isCharging = true;
        
        // 保存原始速度并设置冲刺速度
        float originalSpeed = moveSpeed;
        moveSpeed = chargeSpeed;
        
        // 计算冲向玩家的方向
        Vector3 chargeDirection = (player.position - transform.position).normalized;
        
        // 切换到冲刺状态
        enemy.stateMachine.ChangeState(enemy.dashState);
        
        // 执行冲刺移动
        Move(chargeDirection);
        
        // 设置延迟检查冲刺结果
        Invoke(nameof(CheckChargeResult), 1.5f);
    }
    
    // 检查冲刺结果
    private void CheckChargeResult()
    {
        // 重置速度和状态
        moveSpeed = normalSpeed;
        isCharging = false;
        StopMove();
        
        // 切换回空闲状态
        enemy.stateMachine.ChangeState(enemy.idleState);
        
        // 开始冷却
        StartCooldown();
    }
    
    // 开始冷却
    private void StartCooldown()
    {
        isInCooldown = true;
        Invoke(nameof(EndCooldown), chargeCooldown);
    }
    
    // 结束冷却
    private void EndCooldown()
    {
        isInCooldown = false;
    }
    
    // 随机移动
    private void Wander()
    {
        wanderTimer += Time.deltaTime;
        
        if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0;
            
            // 生成随机位置
            Vector3 randomPos = wanderStartPosition + Random.insideUnitSphere * wanderRadius;
            randomPos.z = transform.position.z; // 保持z轴不变
            
            // 计算移动方向
            Vector3 moveDir = (randomPos - transform.position).normalized;
            
            // 执行移动
            Move(moveDir, wanderInterval);
        }
    }
    
    // 碰撞处理
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
   
            //尝试从碰撞对象获取PlayerMovement组件
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                //计算击退方向（从怪物指向玩家）
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                //调用玩家的击退方法！
                playerMovement.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
            }

            // 取消延迟检查并立即结束冲刺
            CancelInvoke(nameof(CheckChargeResult));
            isCharging = false;
            moveSpeed = normalSpeed;
            StopMove();
            
            // 切换状态并开始冷却
            enemy.stateMachine.ChangeState(enemy.idleState);
            StartCooldown();
        }
    }
    
    // 在编辑器中显示检测范围和徘徊范围
    private void OnDrawGizmosSelected()
    {
        // 显示检测范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        
        // 显示徘徊范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
