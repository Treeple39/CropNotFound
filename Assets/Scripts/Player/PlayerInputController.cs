using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public float RotationInput { get; private set; }
    public bool IsAccelerating { get; private set; }

    [Header("Rotation Settings")]
    public float rotationSpeed = 150f; // 增加默认值

    void Update()
    {
        RotationInput = Input.GetAxis("Horizontal");
        IsAccelerating = Input.GetKey(KeyCode.Space);
    }
}