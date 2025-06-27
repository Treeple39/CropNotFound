using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("移动设置")]
    protected float _moveSpeed = 5f;  // 默认移动速度，子类可以修改
    public bool canMove = true;
    
    // 移动状态变量
    protected Vector3 targetPosition;
    protected Vector3 moveDirection;
    protected float moveDuration;
    protected float moveTimer;
    protected bool isMoving = false;
    protected Vector3 startPosition;
    
    // 移动速度属性，允许子类重写
    public virtual float moveSpeed
    {
        get { return _moveSpeed; }
        protected set { _moveSpeed = value; }
    }
    
    // 是否正在移动的属性
    public bool IsMoving
    {
        get { return isMoving; }
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // 在Update中执行平滑移动
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            
            // 使用Lerp进行插值平滑移动
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            // 检查是否完成移动
            if (t >= 1.0f)
            {
                // 确保位置准确
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
    
    public virtual void Move(Vector3 direction, float moveTime)
    {
        if (!canMove) return;
        
        // 使用移动时间而不是距离
        moveDirection = direction.normalized;
        moveDuration = moveTime;
        
        // 如果moveTime为0，则立即移动；否则开始平滑移动
        if (moveTime <= 0)
        {
            // 立即移动
            Vector3 movement = moveDirection * moveSpeed;
            transform.position += movement;
        }
        else
        {
            // 设置移动参数
            startPosition = transform.position;
            targetPosition = startPosition + moveDirection * moveSpeed * moveTime;
            moveTimer = 0f;
            isMoving = true;
        }
    }
    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞的物体是否可移动
        if (!collision.gameObject.GetComponent<BaseMovement>())
        {
            // 将不可移动物体改为可移动物体
            MakeMovable(collision.gameObject);
        }
    }
    
    protected virtual void MakeMovable(GameObject gameObject)
    {
        // 添加BaseMovement组件使物体可移动
        BaseMovement movement = gameObject.AddComponent<BaseMovement>();
        movement.canMove = true;
        
        Debug.Log($"物体 {gameObject.name} 已变为可移动状态");
    }
}
