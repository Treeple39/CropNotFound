using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class InputManager : Singleton<InputManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // 输入状态
    public Vector2 MouseWorldPosition { get; private set; }
    public bool DashPressed;
    public bool IsMovementEngaged { get; private set; }
    public Vector2 JoystickDirection { get; private set; } = Vector2.zero;

    // 配置参数
    [Header("按键设置")]
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("触屏设置")]
    public float touchDeadZone = 50f;
    public float maxTouchDistance = 300f;
    public float joystickScale = 4.4f;

    [Header("输入方式")]
    public InputMode inputMode = InputMode.FullScreenTouch;

    [Header("虚拟摇杆引用")]
    public Joystick virtualJoystick;

    [HideInInspector] public UI_PreventAccidentalTouch uiTouchManager;

    private Camera mainCamera;
    private Vector2 simulatedTouchPosition;
    private Vector2 initialSimulatePosition;

    private int? movementFingerId = null;

    public void Init()
    {
        mainCamera = Camera.main;
        uiTouchManager = UIManager.Instance.GetComponent<UI_PreventAccidentalTouch>();

        if (Application.isMobilePlatform)
        {
            SystemChangeMode(InputMode.VirtualJoystick);
        }
    }

    public void SystemChangeMode(InputMode mode)
    {
        if(inputMode == mode)
        {
            return;
        }

        inputMode = mode;

        switch (mode)
        {
            case InputMode.FullScreenTouch:
                UIManager.Instance.joystickCanBeActive = false;
                break;
            case InputMode.VirtualJoystick:
                UIManager.Instance.joystickCanBeActive = true;
                break;
            default:
                break;
        }

        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            UIManager.Instance.RefreshJoystick();
        }
        
    }

    private void OnEnable()
    {
        EventHandler.OnInputModeChanged += SystemChangeMode;
    }

    private void OnDisable()
    {
        EventHandler.OnInputModeChanged -= SystemChangeMode;
    }

    void Update()
    {
        mainCamera = Camera.main;
        ResetInputStates();
        UpdateInput();
    }

    private void ResetInputStates()
    {
        DashPressed = false;
    }

    private void UpdateInput()
    {
        if (inputMode == InputMode.VirtualJoystick)
        {
            HandleJoystickInput();
        }
        else
        {
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
        }
    }

    private void HandleMouseInput()
    {
        MouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        bool shouldBlock = uiTouchManager != null && uiTouchManager.ShouldBlockCharacterMovement;

        if (!shouldBlock)
        {
            DashPressed = Input.GetMouseButtonDown(1) || Input.GetKeyDown(dashKey);
            IsMovementEngaged = Input.GetMouseButton(0);
        }
        else
        {
            IsMovementEngaged = false;
        }
    }

    private void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            bool shouldBlock = uiTouchManager?.ShouldBlockCharacterMovement ?? false;

            if (shouldBlock)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    IsMovementEngaged = false;
                    movementFingerId = null;
                }
                continue;
            }

            if (touch.phase == TouchPhase.Began && !movementFingerId.HasValue)
            {
                movementFingerId = touch.fingerId;
                IsMovementEngaged = true;
                MouseWorldPosition = mainCamera.ScreenToWorldPoint(touch.position);
            }
            else if (touch.fingerId == movementFingerId)
            {
                MouseWorldPosition = mainCamera.ScreenToWorldPoint(touch.position);
                IsMovementEngaged = touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
                if (!IsMovementEngaged) movementFingerId = null;
            }

            if (touch.phase == TouchPhase.Began &&
                movementFingerId.HasValue &&
                touch.fingerId != movementFingerId)
            {
                DashPressed = true;
            }
        }
    }

    private void HandleJoystickInput()
    {
        bool shouldBlock = uiTouchManager?.ShouldBlockCharacterMovement ?? false;

        Vector2 inputDir = virtualJoystick != null ? virtualJoystick.Direction * joystickScale : Vector2.zero;

        IsMovementEngaged = !shouldBlock && inputDir.magnitude > 0.1f;

        JoystickDirection = IsMovementEngaged ? inputDir : Vector2.zero;
    }


    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
    }

    public bool IsUsingJoystick()
    {
        return inputMode == InputMode.VirtualJoystick;
    }
}
