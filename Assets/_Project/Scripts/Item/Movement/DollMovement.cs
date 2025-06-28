using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollMovement : BaseMovement
{
    [SerializeField] private float moveDuration = 2f;     // 初始移动持续时间
    [SerializeField] private float dollSpeed = 3f;        // 娃娃移动速度
    [SerializeField] private float waitTimeBetweenMoves = 1f; // 两次移动间的等待时间
    [SerializeField] private float stunDuration = 0.5f;     // 眩晕持续时间

    private Vector3 moveDirection;                        // 当前移动方向
    private float moveTimer = 0f;                         // 移动计时器
    private bool isFirstMove = true;                      // 是否是初始移动
    private bool isMovingForward = true;                  // 是否正在向前移动
    
    private float waitTimer = 0f;                         // 等待计时器
    private bool isWaiting = false;                       // 是否在等待中
    private bool isStunned = false;                       // 是否处于眩晕状态
    private float stunTimer = 0f;                         // 眩晕计时器

    public Enemy enemy;

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

        moveSpeed = dollSpeed;
        StartCoroutine(DelayedInit());
        StartNewMovement();
    }
    
    private IEnumerator DelayedInit()
    {
        yield return null;
        if (enemy.idleState == null || enemy.fleeState == null)
        {
            Debug.LogError("Enemy状态未初始化!");
            yield break;
        }
        enemy.stateMachine.ChangeState(enemy.fleeState);
    }

    protected override void Update()
    {
        base.Update();
        
        // 处理眩晕状态
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
                StartNewMovement();
                enemy.stateMachine.ChangeState(enemy.fleeState);
            }
            return;
        }
        
        // 处理等待状态
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeBetweenMoves)
            {
                isWaiting = false;
                enemy.stateMachine.ChangeState(enemy.fleeState);
                MoveInCurrentDirection();
            }
            return;
        }
        
        // 处理移动状态
        moveTimer += Time.deltaTime;
        float currentMoveDuration = isFirstMove ? moveDuration : 2 * moveDuration;
        
        if (moveTimer >= currentMoveDuration)
        {
            StopMove();
            
            // 切换移动方向
            if (isFirstMove)
            {
                isFirstMove = false;
                isMovingForward = false;
            }
            else
            {
                isMovingForward = !isMovingForward;
            }
            
            moveTimer = 0f;
            isWaiting = true;
            waitTimer = 0f;
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
    
    // 开始新的移动
    private void StartNewMovement()
    {
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0
        ).normalized;
        
        moveTimer = 0f;
        isFirstMove = true;
        isMovingForward = true;
        MoveInCurrentDirection();
    }
    
    // 按当前方向移动
    private void MoveInCurrentDirection()
    {
        if (!gameObject.activeSelf || isStunned) return;
        Move(isMovingForward ? moveDirection : -moveDirection);
    }
    
    // 碰撞处理
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        StopMove();
        isStunned = true;
        stunTimer = stunDuration;
        isWaiting = false;
        enemy.stateMachine.ChangeState(enemy.idleState);
    }
}
