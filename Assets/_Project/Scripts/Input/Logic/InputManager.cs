using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using UnityEngine.UI;


public class InputManager : Singleton<InputManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }


    public Vector2 MouseWorldPosition { get; private set; }
    public bool DashPressed;
    public bool IsMovementEngaged { get; private set; }
    public Vector2 JoystickDirection { get; private set; } = Vector2.zero;


    [Header("��������")]
    public KeyCode dashKey = KeyCode.LeftShift;
    public bool dashButtonClicked = false;
    public Button dashButton;

    [Header("��������")]
    public float touchDeadZone = 50f;
    public float maxTouchDistance = 300f;
    public float joystickScale = 4.4f;

    [Header("���뷽ʽ")]
    public InputMode inputMode = InputMode.FullScreenTouch;

    [Header("����ҡ������")]
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
        if (inputMode == mode)
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

        if (SceneManager.GetActiveScene().name == "MainScene")
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
        HandleDashInput();
    }

    private void HandleMouseInput()
    {
        MouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void HandleDashInput()
    {
        bool shouldBlock = uiTouchManager != null && uiTouchManager.ShouldBlockCharacterMovement;

        if (!shouldBlock)
        {
            DashPressed = Input.GetMouseButtonDown(1) || Input.GetKeyDown(dashKey) || dashButtonClicked;
            dashButtonClicked = false;
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

            // if (touch.phase == TouchPhase.Began &&
            //     movementFingerId.HasValue &&
            //     touch.fingerId != movementFingerId)
            // {
            //     DashPressed = true;
            //     Debug.Log("Touch Dash");
            // }
        }
    }

    private void HandleJoystickInput()
    {
        bool shouldBlock = uiTouchManager?.ShouldBlockCharacterMovement ?? false;

        Vector2 inputDir = virtualJoystick != null ? virtualJoystick.Direction * joystickScale : Vector2.zero;

        IsMovementEngaged = !shouldBlock && inputDir.magnitude > 0.1f;

        JoystickDirection = IsMovementEngaged ? inputDir : Vector2.zero;
        Debug.Log($"Joystick direction: {virtualJoystick.Direction}");
    }


    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
    }

    public bool IsUsingJoystick()
    {
        return inputMode == InputMode.VirtualJoystick;
    }

    public void OnDashButtonClick()
    {

        dashButtonClicked = true;
    }
    public void TryBindDashButton(Button dashButton)
    {
        dashButton.onClick.RemoveAllListeners();
        dashButton.onClick.AddListener(OnDashButtonClick);
        Debug.Log("[InputManager] Dash Button successfully bound.");
    }

    private IEnumerator WaitAndBindDashButton(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject dashButtonObj = GameObject.Find("DashIcon");
        if (dashButtonObj != null)
        {
            Button dashButton = dashButtonObj.GetComponent<Button>();
            if (dashButton != null)
            {
                dashButton.onClick.RemoveAllListeners();
                dashButton.onClick.AddListener(OnDashButtonClick);
                Debug.Log("[InputManager] Dash button successfully bound.");
            }
            else
            {
                Debug.LogWarning("[InputManager] Found GameObject 'DashButton' but no Button component.");
            }
        }
        else
        {
            Debug.LogWarning("[InputManager] DashButton not found in scene.");
        }
    }
}
