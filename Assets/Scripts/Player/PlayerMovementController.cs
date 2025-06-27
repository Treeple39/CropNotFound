using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float maxVelocity = 10f;
    public float acceleration = 5f; // 提高加速度
    public float deceleration = 8f; // 提高减速度
    
    private Rigidbody2D rb;
    private PlayerInputController inputController;
    private Vector2 currentVelocity;
    private Vector2 previousPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<PlayerInputController>();
        previousPosition = rb.position;
    }
    
    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
        previousPosition = rb.position; // 记录前一帧位置
    }
    
    private void HandleRotation()
    {
        float rotationAmount = -inputController.RotationInput * inputController.rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotationAmount);
    }
    
    private void HandleMovement()
    {
        Vector2 forwardDirection = transform.up;
        
        if (inputController.IsAccelerating)
        {
            Vector2 targetVelocity = forwardDirection * moveSpeed;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            
            if (currentVelocity.magnitude > maxVelocity)
            {
                currentVelocity = currentVelocity.normalized * maxVelocity;
            }
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        
        rb.velocity = currentVelocity;
    }
    
    // 获取实际移动速度（用于轨迹系统）
    public Vector2 GetVelocity()
    {
        return (rb.position - previousPosition) / Time.fixedDeltaTime;
    }
}