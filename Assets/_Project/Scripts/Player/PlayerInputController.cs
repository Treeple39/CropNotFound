// PlayerInputController.cs
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public float RotationInput { get; private set; }
    public bool  IsAccelerating { get; private set; }
    public bool  DashPressed    { get; private set; }

    [Header("Rotation Settings")]
    public float   rotationSpeed = 150f; 
    [Header("Dash Settings")]
    public KeyCode dashKey       = KeyCode.LeftShift; // 冲刺键

    void Update()
    {
        RotationInput   = Input.GetAxis("Horizontal");
        IsAccelerating  = Input.GetKey(KeyCode.Space);
        DashPressed     = Input.GetKeyDown(dashKey);
    }
}