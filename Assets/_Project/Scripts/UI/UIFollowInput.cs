using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIFollowInput : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Vector2 followIntensity = new Vector2(0.1f, 0.1f);
    [SerializeField] private Vector2 maxOffset = new Vector2(50f, 50f);
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Input Mode")]
    [SerializeField] private bool useDeviceTilt = false;
    [SerializeField] private float tiltSensitivity = 2f;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private Vector2 velocity = Vector2.zero;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalAnchoredPosition = rectTransform.anchoredPosition;
        parentCanvas = GetComponentInParent<Canvas>();

        if (SystemInfo.supportsAccelerometer)
        {
            Input.gyro.enabled = useDeviceTilt;
        }
        else if (useDeviceTilt)
        {
            Debug.LogWarning("Accelerometer not supported. Using mouse input.");
            useDeviceTilt = false;
        }
    }

    private void Update()
    {
        Vector2 inputPosition = GetInputPosition();
        Vector2 targetOffset = CalculateTargetOffset(inputPosition);

        rectTransform.anchoredPosition = Vector2.SmoothDamp(
            rectTransform.anchoredPosition,
            originalAnchoredPosition + targetOffset,
            ref velocity,
            smoothTime
        );
    }

    private Vector2 GetInputPosition()
    {
        if (useDeviceTilt)
        {
            // 使用重力感应输入
            Vector2 tilt = Input.acceleration * tiltSensitivity;
            return new Vector2(tilt.x, tilt.y);
        }
        else
        {
            // 使用鼠标输入
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.worldCamera,
                out mousePos
            );
            return mousePos;
        }
    }

    private Vector2 CalculateTargetOffset(Vector2 inputPosition)
    {
        // 计算相对于原点的偏移
        Vector2 offset = inputPosition - originalAnchoredPosition;

        // 分别应用XY轴的跟随强度
        offset.x *= followIntensity.x;
        offset.y *= followIntensity.y;

        // 分别限制XY轴的最大偏移
        offset.x = Mathf.Clamp(offset.x, -maxOffset.x, maxOffset.x);
        offset.y = Mathf.Clamp(offset.y, -maxOffset.y, maxOffset.y);

        return offset;
    }

    public void SetUseDeviceTilt(bool useTilt)
    {
        if (SystemInfo.supportsAccelerometer || !useTilt)
        {
            useDeviceTilt = useTilt;
            Input.gyro.enabled = useDeviceTilt;
        }
        else
        {
            Debug.LogWarning("Cannot enable tilt input - accelerometer not supported.");
            useDeviceTilt = false;
        }
    }

    // 重置到原始位置
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalAnchoredPosition;
        velocity = Vector2.zero;
    }
}