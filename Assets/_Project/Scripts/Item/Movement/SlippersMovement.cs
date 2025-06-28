using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderEnemy : BaseMovement
{
    [Header("Ѳ������")]
    public float wanderRadius = 5f;    // Ѳ�߰뾶
    public float wanderInterval = 1.5f;  // Ѳ�߼��
    private float wanderTimer;
    private Vector3 wanderCenter;     // Ѳ�����ĵ�

    [Header("�������")]
    public float chargeSpeed = 12f;    // ����ٶ�
    public float chargeDistance = 7f;  // ������
    public float chargeCooldown = 5f;  // ������ȴʱ��
    private bool isInCooldown = false;
    private Vector3 chargeTarget;      // ���Ŀ���
    private bool isCharging = false;

    [Header("��Ҽ��")]
    public float detectRange = 8f;     // ������ҵľ���
    public LayerMask playerLayer;      // ��Ҳ㼶
    private Transform player;          // �������
    private Vector3 detectedPlayerPos;

    #region ״̬��
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
            Debug.LogError("ȱ��Enemy���!", gameObject);
            enabled = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        wanderTimer = wanderInterval;
        wanderCenter = transform.position; // ��ʼ��Ѳ������
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;

        if (enemy.idleState == null || enemy.dashState == null)
        {
            Debug.LogError("Enemy״̬δ��ʼ��!");
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

    // ��ʼ���
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

    // ���������ж�
    private void CheckChargeResult()
    {
        isCharging = false;
        moveSpeed = _moveSpeed;
        enemy.stateMachine.ChangeState(enemy.idleState);

        StartCooldown();
    }

    // ������ȴ״̬
    private void StartCooldown()
    {
        isInCooldown = true;
        Invoke(nameof(ResetCooldown), chargeCooldown);
    }

    // ������ȴ
    private void ResetCooldown()
    {
        isInCooldown = false;
    }

    // ���Ѳ��
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

    // ��ײ���
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            // ������Һ�����ֹͣ���
            CancelInvoke(nameof(CheckChargeResult));
            enemy.stateMachine.ChangeState(enemy.idleState);
            StartCooldown();
        }
    }
}