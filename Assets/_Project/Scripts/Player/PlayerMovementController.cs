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
        // 发起冲刺：当接收到冲刺请求、不在冲刺中且冷却结束
        if (dashRequested && !isDashing && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            // 直接在冲刺开始时设置一次速度
            currentVelocity = (Vector2)transform.up * dashSpeed;
            rb.velocity = currentVelocity;
        }

        // 【修改】在 FixedUpdate 的开头就消耗掉冲刺请求，无论成功与否
        // 这样可以确保请求只被处理一次
        dashRequested = false;

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;

                // 【修改】冲刺结束后，重置 currentVelocity
                // 这样 HandleMovement 会从当前实际速度（可能为0）开始计算，而不是从冲刺速度开始减速
                // 如果希望保留冲刺后的惯性，可以设置为 rb.velocity
                // 如果希望冲刺后立即停下并可控，可以设置为 Vector2.zero
                currentVelocity = Vector2.zero;
            }
            // 【修改】冲刺期间，速度已经在开始时设置好了，物理引擎会维持它。
            // 除非你想抵抗外力，否则不需要每一帧都设置。这里我们保持简单，只在开始时设置。
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