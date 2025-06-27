using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("移动设置")]
    protected float _moveSpeed = 5f;  // 默认移动速度，子类可以修改
    public bool canMove = true;
    
    // 移动速度属性，允许子类重写
    public virtual float moveSpeed
    {
        get { return _moveSpeed; }
        protected set { _moveSpeed = value; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Move(Vector3 direction, float moveTime)
    {
        if (!canMove) return;
        
        // 使用移动时间而不是距离
        // 如果moveTime为0，则立即移动；否则使用协程在指定时间内平滑移动
        if (moveTime <= 0)
        {
            // 立即移动
            Vector3 movement = direction.normalized * moveSpeed;
            transform.position += movement;
        }
        else
        {
            // 开启协程进行平滑移动
            StartCoroutine(SmoothMove(direction, moveTime));
        }
    }
    
    // 平滑移动协程
    protected virtual IEnumerator SmoothMove(Vector3 direction, float moveTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction.normalized * moveSpeed * moveTime;
        float elapsedTime = 0;
        
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终位置准确
        transform.position = targetPosition;
    }
    
    private void OnCollisionEnter(Collision collision)
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
