using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;

public class BaseMovement : MonoBehaviour
{
    protected float _moveSpeed = 5f;  // 默认移动速度，子类可以修改
    public bool canMove = true;

    protected Rigidbody2D rb;

    // 移动状态变量
    protected Vector3 targetPosition;
    public Vector3 moveDirection;
    protected float moveDuration;
    protected float moveTimer;
    protected bool isMoving = false;
    protected Vector3 startPosition;
    protected bool isContinuousMoving = false;
    public bool facingright = true;

    // 移动速度属性，允许子类重写
    public virtual float moveSpeed
    {
        get { return _moveSpeed; }
        protected set { _moveSpeed = value; }
    }

    // 是否正在移动的属性
    public bool IsMoving
    {
        get { return isMoving || isContinuousMoving; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 如果没有Rigidbody2D组件，添加一个
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            // 默认设置，可根据需要调整
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 防止旋转
        }
        facingright = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;

            // 检查是否达到指定的移动时间
            if (moveTimer >= moveDuration)
            {
                // 移动时间结束，停止移动
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
                isMoving = false;
            }

        }

    }




    protected virtual void FixedUpdate()
    {
        if (isContinuousMoving && canMove && rb != null)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    public virtual void Move(Vector3 direction)
    {
        if (!canMove) return;

        moveDirection = direction.normalized;
        isContinuousMoving = true;
        isMoving = false; // 停止任何正在进行的定时移动

        // 直接设置速度
        if (rb != null)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    public virtual void Move(Vector3 direction, float moveTime)
    {
        if (!canMove) return;

        // 停止连续移动
        isContinuousMoving = false;

        // 设置移动参数
        moveDirection = direction.normalized;
        moveDuration = moveTime;
        moveTimer = 0f;

        // 直接设置速度，让物体按指定方向和速度移动
        if (rb != null)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
            isMoving = true; // 启动计时器，在指定时间后停止移动
        }
    }

    public virtual void StopMove()
    {
        isContinuousMoving = false;
        isMoving = false;

        // 停止Rigidbody2D的运动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞的物体是否存在BaseMovement组件
        BaseMovement collidedMovement = collision.gameObject.GetComponent<BaseMovement>();
        if (collidedMovement != null && !collidedMovement.canMove && collision.gameObject.tag == "Enemy")
        {
            // 将不可移动物体改为可移动物体
            MakeMovable(collision.gameObject);
        }
    }

    protected virtual void MakeMovable(GameObject gameObject)
    {
        BaseMovement movement = gameObject.GetComponent<BaseMovement>();
        movement.canMove = true;

        Debug.Log($"物体 {gameObject.name} 已变为可移动状态");
    }
}
