using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderEnemy : BaseMovement
{
    [Header("巡逻设置")]
    public float wanderRadius = 5f;    // 巡逻半径
    public float wanderInterval = 1.5f;  // 巡逻间隔
    private float wanderTimer;
    private Vector3 wanderCenter;     // 巡逻中心点

    [Header("冲锋设置")]
    public float chargeSpeed = 12f;    // 冲锋速度
    public float chargeDistance = 7f;  // 冲锋距离
    public float chargeCooldown = 5f;  // 技能冷却时间
    private bool isInCooldown = false;
    private Vector3 chargeTarget;      // 冲锋目标点
    private bool isCharging = false;

    [Header("玩家检测")]
    public float detectRange = 8f;     // 发现玩家的距离
    public LayerMask playerLayer;      // 玩家层级
    private Transform player;          // 玩家引用
    private Vector3 detectedPlayerPos;

    #region 状态机
    public Enemy enemy;
    private bool isInitialized = false;
    #endregion

    protected override void Start()
    {
        base.Start();
        moveSpeed = 3f;
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("缺少Enemy组件!", gameObject);
            enabled = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        wanderTimer = wanderInterval;
        wanderCenter = transform.position; // 初始化巡逻中心
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        if (enemy.idleState == null || enemy.dashState == null)
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

        if (!canMove || isInCooldown) return;

        if (!isInCooldown && !isCharging && DetectPlayer())
        {
            StartCharge();
        }
        else if (!isCharging)
        {
            Wander();
        }
    }

    private bool DetectPlayer()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= detectRange;
    }

    // 开始冲锋
    private void StartCharge()
    {
        isCharging = true;
        detectedPlayerPos = player.position;

        Vector3 chargeDir = (detectedPlayerPos - transform.position).normalized;
        chargeTarget = transform.position + chargeDir * chargeDistance;

        float originalSpeed = moveSpeed;
        moveSpeed = chargeSpeed;

        float chargeTime = chargeDistance / chargeSpeed;
        Move(chargeDir, chargeTime);
        enemy.stateMachine.ChangeState(enemy.dashState);

        Invoke(nameof(CheckChargeResult), chargeTime);
    }

    // 冲锋结束后判定
    private void CheckChargeResult()
    {
        isCharging = false;
        moveSpeed = _moveSpeed;
        enemy.stateMachine.ChangeState(enemy.idleState);

        StartCooldown();
    }

    // 进入冷却状态
    private void StartCooldown()
    {
        isInCooldown = true;
        Invoke(nameof(ResetCooldown), chargeCooldown);
    }

    // 重置冷却
    private void ResetCooldown()
    {
        isInCooldown = false;
    }

    // 随机巡逻
    private void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0;
            Vector3 randomPos = wanderCenter + Random.insideUnitSphere * wanderRadius;
            randomPos.y = transform.position.y; 

            Vector3 moveDir = (randomPos - transform.position).normalized;
            Move(moveDir, wanderInterval);
        }
    }

    // 碰撞检测
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            // 命中玩家后立即停止冲锋
            CancelInvoke(nameof(CheckChargeResult));
            enemy.stateMachine.ChangeState(enemy.idleState);
            StartCooldown();
        }
    }
}