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
        // ������Ļ���ĵ���������
        screenCenter = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane)
        );
    }

    private void Update()
    {
        // ������Ļ�ߴ�仯
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
            // ʹ��������Ӧ���룬����������ϵ��
            Vector3 tilt = Input.acceleration * tiltSensitivity;
            return new Vector3(tilt.x, tilt.y, 0);
        }
        else
        {
            // ʹ��������룬�������Ļ����
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private Vector3 CalculateTargetOffset(Vector3 inputPosition)
    {
        // �����������Ļ���ĵ�ƫ��
        Vector3 offset = inputPosition - screenCenter;

        // �ֱ�Ӧ��XY��ĸ���ǿ��
        offset.x *= followIntensity.x;
        offset.y *= followIntensity.y;

        // �ֱ�����XY������ƫ��
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

    // �������ø���ǿ�ȵķ���
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