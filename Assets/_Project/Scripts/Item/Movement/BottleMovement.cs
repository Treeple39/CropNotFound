using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleMovement : BaseMovement
{
    [SerializeField] private float minStayTime = 3f;      // 最小停留时间（秒）
    [SerializeField] private float maxStayTime = 7f;      // 最大停留时间（秒）
    [SerializeField] private float teleportRadius = 10f;  // 传送半径范围
    [SerializeField] private int maxTeleportAttempts = 10; // 最大传送尝试次数
    [SerializeField] private float minDistance = 0.5f;    // 与其他物体的最小距离
    
    private float stayTimer;                              // 停留计时器
    private float currentStayTime;                        // 当前停留时间
    private Vector3 bottleInitialPosition;                // 初始位置

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
        bottleInitialPosition = transform.position;
        
        // 确保ItemGenerator已初始化
        if (ItemGenerator.Instance == null)
        {
            Debug.LogWarning("ItemGenerator未初始化，位置检测可能无法正常工作");
        }
        
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
        
        // 更新停留计时器
        stayTimer += Time.deltaTime;
         
        // 如果停留时间结束，进行传送
        if (stayTimer >= currentStayTime)
        {
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
            Vector3 randomPosition = bottleInitialPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // 检查新位置是否有重叠
            if (!IsOverlapping(randomPosition))
            {
                // 找到有效位置，进行传送
                transform.position = randomPosition;
                Debug.Log($"瓶子在尝试{attempts}次后传送到了新位置: {randomPosition}");
                return;
            }
        }
        
        Debug.LogWarning($"瓶子尝试{maxTeleportAttempts}次后未能找到有效位置");
    }
    
    // 检查指定位置是否与其他物体重叠
    private bool IsOverlapping(Vector3 position)
    {
        // 如果ItemGenerator未初始化，直接返回false
        if (ItemGenerator.Instance == null)
            return false;
        
        // 遍历所有已生成的物体Transform
        foreach (Transform otherTransform in ItemGenerator.Instance.spawnedTransforms)
        {
            // 跳过自己
            if (otherTransform == transform)
                continue;
                
            // 如果距离小于最小距离，则认为有重叠
            if (Vector3.Distance(position, otherTransform.position) < minDistance)
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
        
        // 额外显示最小距离
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}
