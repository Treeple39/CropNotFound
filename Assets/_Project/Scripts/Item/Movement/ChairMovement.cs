using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMovement : BaseMovement
{
    [SerializeField] private float detectionRadius = 4f;      // 检测玩家的半径
    [SerializeField] private float runAwayDuration = 2f;      // 逃跑持续时间（秒）
    [SerializeField] private float runAwaySpeed = 3f;         // 逃跑速度
    [SerializeField] private Transform player;                // 玩家对象引用
    
    public bool isRunningAway = false;                       // 是否正在逃跑
    private float runAwayTimer = 0f;                          // 逃跑计时器
   
    #region 状态机
    public Enemy enemy;
    private bool isInitialized = false;
    #endregion
    // Start方法覆盖
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

        // 如果未指定玩家，尝试在场景中查找玩家对象
        if (player == null)
        {
            
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogWarning("未找到玩家对象，椅子将无法检测玩家");
            }
           
        }
        // 设置椅子的基础属性
        moveSpeed = runAwaySpeed;

        StartCoroutine(DelayedInit());
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
    }

    // Update方法覆盖
    protected override void Update()
    {
        base.Update();
        
        // 检查玩家并处理逃跑行为
        if (player != null)
        {
            // 检测玩家是否在范围内
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            // 如果玩家在检测范围内且椅子当前不在逃跑状态，开始逃跑
            if (distanceToPlayer <= detectionRadius && !isRunningAway && !IsMoving)
            {
                StartRunningAway();
            }
            
            // 如果正在逃跑，更新逃跑计时
            if (isRunningAway)
            {
                runAwayTimer += Time.deltaTime;
                
                // 如果逃跑时间到，停止逃跑
                if (runAwayTimer >= runAwayDuration)
                {
                    StopRunningAway();
                }
            }
        }
    }
    
    // 开始逃跑
    private void StartRunningAway()
    {
        if (!canMove) return;
        
        enemy.stateMachine.ChangeState(enemy.fleeState);

        isRunningAway = true;
        runAwayTimer = 0f;
        
        // 计算远离玩家的方向
        Vector3 awayFromPlayerDirection = (transform.position - player.position).normalized;
        
        // 使用基类的Move方法移动
        Move(awayFromPlayerDirection, runAwayDuration);
        
        Debug.Log("椅子发现了玩家，开始逃跑！");
    }
    
    // 停止逃跑
    private void StopRunningAway()
    {
        isRunningAway = false;
        runAwayTimer = 0f;
        enemy.stateMachine.ChangeState(enemy.idleState);
        StopMove();

        Debug.Log("椅子停止逃跑");
    }
    
    // 在编辑器中显示检测范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
