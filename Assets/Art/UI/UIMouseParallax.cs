using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIParallaxEffect : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Movement sensitivity (0=no movement)")]
    [SerializeField] private Vector2 sensitivity = new Vector2(0.1f, 0.1f);

    [Tooltip("Initial position offset")]
    [SerializeField] private Vector2 positionOffset = Vector2.zero;

    [Tooltip("Movement multiplier")]
    [SerializeField] private Vector2 moveRatio = new Vector2(1f, 1f);

    [Header("Advanced")]
    [Tooltip("Reference canvas (auto-detected if null)")]
    [SerializeField] private Canvas targetCanvas;

    private RectTransform _rectTransform;
    private Vector2 _originalAnchoredPos;
    private Vector2 _screenCenter;
    private float _scaleFactor;

    private void Reset()
    {
        // Auto-configure based on gameObject name
        switch (gameObject.name)
        {
            case "ui_menu_bg_middle":
                sensitivity = new Vector2(0.05f, 0.03f);
                moveRatio = new Vector2(0.6f, 0.6f);
                break;
            case "ui_menu_soul_front":
                sensitivity = new Vector2(0.15f, 0.1f);
                moveRatio = new Vector2(0.8f, 0.8f);
                break;
            case "ui_menu_title":
                sensitivity = new Vector2(0.25f, 0.2f);
                moveRatio = new Vector2(1.0f, 1.0f);
                break;
            case "ui_menu_touchToStart":
                sensitivity = new Vector2(0.4f, 0.3f);
                moveRatio = new Vector2(1.2f, 1.1f);
                break;
            case "ui_menu_actor_l":
            case "ui_menu_actor_r":
                sensitivity = new Vector2(0.3f, 0.25f);
                moveRatio = new Vector2(1.1f, 1.0f);
                break;
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalAnchoredPos = _rectTransform.anchoredPosition;

        // Auto-detect canvas if not set
        targetCanvas = targetCanvas ? targetCanvas : GetComponentInParent<Canvas>();

        if (targetCanvas == null)
        {
            Debug.LogError("No Canvas found in parent hierarchy!", this);
            enabled = false;
            return;
        }

        CalculateScreenCenter();
    }

    private void OnEnable()
    {
        CalculateScreenCenter();
    }

    private void CalculateScreenCenter()
    {
        if (targetCanvas == null) return;

        _scaleFactor = targetCanvas.scaleFactor > 0 ? targetCanvas.scaleFactor : 1;
        _screenCenter = new Vector2(
            Screen.width / (2f * _scaleFactor),
            Screen.height / (2f * _scaleFactor));
    }

    private void Update()
    {
        if (!targetCanvas) return;

        // Get scaled mouse position
        Vector2 mousePos = Input.mousePosition / _scaleFactor;

        // Calculate normalized offset (-1 to 1 range)
        Vector2 mouseOffset = new Vector2(
            (mousePos.x - _screenCenter.x) / _screenCenter.x,
            (mousePos.y - _screenCenter.y) / _screenCenter.y);

        // Apply parallax effect
        _rectTransform.anchoredPosition = _originalAnchoredPos +
            new Vector2(
                mouseOffset.x * sensitivity.x * moveRatio.x + positionOffset.x,
                mouseOffset.y * sensitivity.y * moveRatio.y + positionOffset.y);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || !isActiveAndEnabled) return;

        // Editor-time preview
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            Awake();
            Update();
        };
    }
#endif
}