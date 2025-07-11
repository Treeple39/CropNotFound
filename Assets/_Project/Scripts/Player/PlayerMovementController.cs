using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMovement : MonoBehaviour
{
    //在角色管理器出来之前，暂时先单例吧
    public static PlayerMovement Instance;

    [Header("直接移动参数")]
    public float maxSpeed = 10f;
    public float deadZoneRadius = 0.5f;
    public float maxThrustRadius = 5f;

    [Header("冲刺参数")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    [Header("特效")]
    public GameObject dashEffectPrefab;
    public Sprite speedChangeChSprite;
    
    [Header("UI交互")]
    [Tooltip("UI防误触管理器")]
    public UI_PreventAccidentalTouch uiTouchManager;

    private Rigidbody2D rb;
    private PlayerInputController inputController;

    private bool isDashing;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 1.0f;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<PlayerInputController>();
        
        // 如果没有通过Inspector指定UI防误触管理器，尝试在场景中查找
        if (uiTouchManager == null)
        {
            uiTouchManager = FindObjectOfType<UI_PreventAccidentalTouch>();
        }
    }

    void Update()
    {
        HandleTimersAndInput();
    }

    private void OnEnable()
    {
        EventHandler.OnChangeSpeed += OnBoostPlayerSpeed;
    }

    private void OnDisable()
    {
        EventHandler.OnChangeSpeed -= OnBoostPlayerSpeed;
    }

    private void OnBoostPlayerSpeed(float speedBoostMultiplier, float speedBoostDuration)
    {
        UpdateMaxVelocity(speedBoostMultiplier);

        ItemUIData messageData = new ItemUIData
        {
            messageImage = speedChangeChSprite,
            message = $"速度提升 {speedBoostMultiplier}倍! 持续 {speedBoostDuration}秒,冲啊！",
            messageID = -1,
        };

        EventHandler.CallMessageShow(messageData);
        StartCoroutine(ResetSpeed(speedBoostMultiplier, speedBoostDuration));
    }

    private IEnumerator ResetSpeed(float speedBoostMultiplier, float speedBoostDuration)
    {
        yield return new WaitForSecondsRealtime(speedBoostDuration);
        UpdateMaxVelocity(1/speedBoostMultiplier);
    }


    public float DashCooldownProgress
    {
        get
        {
            if (dashCooldown <= 0f) return 1.0f;
            return Mathf.Clamp01(1.0f - (dashCooldownTimer / dashCooldown));
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDashingState();
        }
        else
        {
            // 检查是否应该阻止角色移动（UI防误触）
            if (uiTouchManager != null && uiTouchManager.ShouldBlockCharacterMovement)
            {
                // 如果应该阻止移动，不执行移动逻辑
                return;
            }
            
            // ★★★★★【核心修改】★★★★★
            // 只有在玩家按住鼠标左键时，才执行引导逻辑
            if (inputController.IsMovementEngaged)
            {
                HandleInstantRotation();
                HandleDirectMovement();
            }
            // 当松开鼠标时，我们不执行任何操作。
            // Rigidbody2D的Linear Drag属性会自动让飞船平滑地减速停下。
            // ★★★★★★★★★★★★★★★★★★★
        }
    }

    private void HandleDirectMovement()
    {
        Vector2 directionVector = inputController.MouseWorldPosition - rb.position;
        float distanceToMouse = directionVector.magnitude;
        float speedRatio = 0f;

        if (distanceToMouse > deadZoneRadius)
        {
            speedRatio = Mathf.InverseLerp(deadZoneRadius, maxThrustRadius, distanceToMouse);
            speedRatio = Mathf.Clamp01(speedRatio);
        }

        float desiredSpeed = maxSpeed * speedRatio;
        Vector2 newVelocity = directionVector.normalized * desiredSpeed;
        rb.velocity = newVelocity;
    }

    private void HandleInstantRotation()
    {
        Vector2 direction = inputController.MouseWorldPosition - rb.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = targetAngle;
        }
    }

    // --- 冲刺和计时器逻辑 (保持不变) ---
    private void HandleTimersAndInput()
    {
        if (dashCooldownTimer > 0f) { dashCooldownTimer -= Time.deltaTime; }
        if (inputController.DashPressed && !isDashing && dashCooldownTimer <= 0f) { StartDash(); }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        Vector2 dashDirection = transform.up;
        rb.velocity = dashDirection * dashSpeed;
        if (dashEffectPrefab != null)
        {
            Vector2 effectDirection = -dashDirection;
            float angle = Mathf.Atan2(effectDirection.y, effectDirection.x) * Mathf.Rad2Deg;
            Quaternion effectRotation = Quaternion.Euler(0, 0, angle + 90f);
            Instantiate(dashEffectPrefab, transform.position, effectRotation);
        }
    }

    private void HandleDashingState()
    {
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f) { isDashing = false; }
    }

    public void UpdateMaxVelocity(float rate)
    {
        maxSpeed *= rate;
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }
}