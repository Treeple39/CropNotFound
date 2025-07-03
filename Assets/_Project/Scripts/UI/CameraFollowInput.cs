using UnityEngine;

public class CameraFollowInput : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Vector2 followIntensity = new Vector2(0.1f, 0.1f);
    [SerializeField] private Vector2 maxOffset = new Vector2(1.5f, 1.5f);
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Input Mode")]
    [SerializeField] private bool useDeviceTilt = false;
    [SerializeField] private float tiltSensitivity = 2f;

    private Vector3 originalPosition;
    private Vector3 screenCenter;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        originalPosition = transform.position;
        CalculateScreenCenter();

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

    private void CalculateScreenCenter()
    {
        // 计算屏幕中心的世界坐标
        screenCenter = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane)
        );
    }

    private void Update()
    {
        // 处理屏幕尺寸变化
        if (Screen.width != Screen.width || Screen.height != Screen.height)
        {
            CalculateScreenCenter();
        }

        Vector3 inputPosition = GetInputPosition();
        Vector3 targetOffset = CalculateTargetOffset(inputPosition);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            originalPosition + targetOffset,
            ref velocity,
            smoothTime
        );
    }

    private Vector3 GetInputPosition()
    {
        if (useDeviceTilt)
        {
            // 使用重力感应输入，乘以灵敏度系数
            Vector3 tilt = Input.acceleration * tiltSensitivity;
            return new Vector3(tilt.x, tilt.y, 0);
        }
        else
        {
            // 使用鼠标输入，相对于屏幕中心
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private Vector3 CalculateTargetOffset(Vector3 inputPosition)
    {
        // 计算相对于屏幕中心的偏移
        Vector3 offset = inputPosition - screenCenter;

        // 分别应用XY轴的跟随强度
        offset.x *= followIntensity.x;
        offset.y *= followIntensity.y;

        // 分别限制XY轴的最大偏移
        offset.x = Mathf.Clamp(offset.x, -maxOffset.x, maxOffset.x);
        offset.y = Mathf.Clamp(offset.y, -maxOffset.y, maxOffset.y);
        offset.z = 0;

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

    // 单独设置跟随强度的方法
    public void SetFollowIntensity(Vector2 intensity)
    {
        followIntensity = intensity;
    }

    public void SetFollowIntensityX(float intensityX)
    {
        followIntensity.x = intensityX;
    }

    public void SetFollowIntensityY(float intensityY)
    {
        followIntensity.y = intensityY;
    }
}