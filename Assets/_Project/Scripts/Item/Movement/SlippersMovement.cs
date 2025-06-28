using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlippersMovement : BaseMovement
{
    [Header("????")]
    public float wanderRadius = 5f;    // ????
    public float wanderInterval = 1.5f;  // ????
    private float wanderTimer;
    private Vector3 wanderCenter;     // ?????

    [Header("????")]
    public float chargeSpeed = 12f;    // ????
    public float chargeDistance = 7f;  // ????
    public float chargeCooldown = 5f;  // ??????
    private bool isInCooldown = false;
    private Vector3 chargeTarget;      // ?????
    private bool isCharging = false;

    [Header("????")]
    public float detectRange = 6f;     // ???????
    public LayerMask playerLayer;      // ????
    private Transform player;          // ??Transform
    private Vector3 detectedPlayerPos;

    #region ???
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
            Debug.LogError("????Enemy??!", gameObject);
            enabled = false;
            return;
        }
        
        // ??????????
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            player = GameObject.Find("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("????????! ??????Player??", gameObject);
            }
            else
            {
                Debug.Log("????????");
            }
        }
        else
        {
            Debug.Log("????????");
        }
        
        // ????????
        if (playerLayer == 0)
        {
            playerLayer = LayerMask.GetMask("Player");
            Debug.Log("????????: " + playerLayer);
        }
        
        wanderTimer = wanderInterval;
        wanderCenter = transform.position; // ????????
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        if (enemy.idleState == null || enemy.dashState == null)
        {
            Debug.LogError("Enemy??????!");
            yield break;
        }
        isInitialized = true;
        enemy.stateMachine.ChangeState(enemy.idleState);
    }

    protected override void Update()
    {
        base.Update();

        if (!canMove || !isInitialized) return;
        
        // ????????
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        if (!isInCooldown && !isCharging && DetectPlayer())
        {
            Debug.Log("?????????????");
            StartCharge();
        }
        else if (!isCharging)
        {
            Wander();
        }
    }

    private bool DetectPlayer()
    {
        if (player == null) 
        {
            Debug.LogWarning("???????????");
            return false;
        }
        
        float distance = Vector3.Distance(transform.position, player.position);
        bool detected = distance <= detectRange;
        
        // ???????????
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"????: ??={distance}, ??={detectRange}, ????={detected}");
        }
        
        return detected;
    }

    // ????
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
        
        // ?????????
        if (enemy != null && enemy.dashState != null && enemy.stateMachine != null)
        {
            enemy.stateMachine.ChangeState(enemy.dashState);
            Debug.Log("?????????");
        }
        else
        {
            Debug.LogError("??????????????????");
        }

        Invoke(nameof(CheckChargeResult), chargeTime);
    }

    // ?????????
    private void CheckChargeResult()
    {
        isCharging = false;
        moveSpeed = _moveSpeed;
        
        // ?????????
        if (enemy != null && enemy.idleState != null && enemy.stateMachine != null)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
            Debug.Log("?????????????");
        }

        StartCooldown();
    }

    // ??????
    private void StartCooldown()
    {
        isInCooldown = true;
        Debug.Log($"?????????{chargeCooldown}???????");
        Invoke(nameof(ResetCooldown), chargeCooldown);
    }

    // ??????
    private void ResetCooldown()
    {
        isInCooldown = false;
        Debug.Log("?????????????");
    }

    // ????
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

    // ????
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("????????");
            // ???????????
            CancelInvoke(nameof(CheckChargeResult));
            enemy.stateMachine.ChangeState(enemy.idleState);
            StartCooldown();
        }
    }
    
    // ???????????
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}