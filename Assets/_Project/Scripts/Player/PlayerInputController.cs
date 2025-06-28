using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    // --- 对外公开的输入状态 ---
    public Vector2 MouseWorldPosition { get; private set; }
    public bool DashPressed { get; private set; }
    public bool IsMovementEngaged { get; private set; } // ★★★ NEW ★★★

    // --- 配置 ---
    [Header("按键设置")]
    public KeyCode dashKey = KeyCode.LeftShift;

    // --- 内部变量 ---
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. 鼠标位置 (始终更新)
        MouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. 冲刺输入 (瞬间按下)
        DashPressed = Input.GetMouseButtonDown(1) || Input.GetKeyDown(dashKey);

        // 3. 移动引导输入 (持续按住)
        IsMovementEngaged = Input.GetMouseButton(0); 
    }
}