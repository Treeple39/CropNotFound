using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float maxVelocity = 10f;
    public float acceleration = 5f;
    public float deceleration = 8f;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;       // 冲刺速度
    public float dashDuration = 0.2f;   // 冲刺持续时间
    public float dashCooldown = 1.0f;   // 冲刺冷却时间
    // public KeyCode dashKey; // 【修改】从 InputController 直接获取，不再需要此字段

    [Header("Effects")]
    public GameObject dashEffectPrefab;

    private Rigidbody2D rb;
    private PlayerInputController inputController;
    private Vector2 currentVelocity;
    private Vector2 previousPosition;

    // Dash 内部状态
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;

    private bool dashRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<PlayerInputController>();
    }
    void Start()
    {
        previousPosition = rb.position;
    }

    void Update()
    {
        // 1) 【修改】在 Update 中检测输入，并设置“请求”标志
        // 如果已经有请求了，就不要再检测，防止一帧内多次请求（虽然不太可能）
        if (!dashRequested)
        {
            // 【修改】直接从 inputController 读取按键，支持运行时更改
            if (Input.GetKeyDown(inputController.dashKey))
            {
                dashRequested = true;
            }
        }

        // 2) 冷却计时，用 Time.deltaTime，这部分逻辑是正确的
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleDash();      // 优先处理冲刺，因为它会覆盖移动
        HandleMovement();  // 只有在不冲刺时才处理普通移动
        previousPosition = rb.position;
    }

    private void HandleRotation()
    {
        float rot = -inputController.RotationInput
                    * inputController.rotationSpeed
                    * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rot);
    }

    private void HandleDash()
    {
        if (dashRequested && !isDashing && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            Vector2 dashDirection = transform.up;
            currentVelocity = dashDirection * dashSpeed;
            rb.velocity = currentVelocity;

            if (dashEffectPrefab != null)
            {
                // 1. 计算特效的位置 (只改变XY, Z使用预制体的值)
                Vector3 effectPosition = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    dashEffectPrefab.transform.position.z
                );
                // 1. 获取期望的方向向量
                Vector2 effectDirection = -dashDirection;

                // 2. 使用 Atan2 将方向向量转换为世界角度（弧度），再转换为度
                float worldAngle = Mathf.Atan2(effectDirection.y, effectDirection.x) * Mathf.Rad2Deg;

                // 3. 根据你的预制体特性，应用映射公式来计算最终的X轴旋转值
                float rotationX = -worldAngle;



                // 5. 使用计算出的正确角度来创建最终的旋转
                Quaternion finalRotation = Quaternion.Euler(rotationX, 0, 0);

                // 3. 使用 Instantiate 创建特效
                Instantiate(dashEffectPrefab, effectPosition, finalRotation);
            }
        }

        dashRequested = false;

        // 【修改】冲刺结束时不再需要手动停止特效，因为它会自己销毁
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                currentVelocity = Vector2.zero;
            }
        }
    }

    private void HandleMovement()
    {
        // 如果正在冲刺，就跳过常规移动逻辑
        if (isDashing)
            return;

        Vector2 forward = transform.up;
        if (inputController.IsAccelerating)
        {
            Vector2 target = forward * moveSpeed;
            currentVelocity = Vector2.Lerp(
                currentVelocity, target, acceleration * Time.fixedDeltaTime);
            currentVelocity = Vector2.ClampMagnitude(
                currentVelocity, maxVelocity);
        }
        else
        {
            currentVelocity = Vector2.Lerp(
                currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.velocity = currentVelocity;
    }

    public Vector2 GetVelocity()
    {
        return (rb.position - previousPosition) / Time.fixedDeltaTime;
    }
}