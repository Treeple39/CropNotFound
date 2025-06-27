using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : BaseMovement
{
    [Header("立方体移动设置")]
    private bool isMoving = false;         // 是否正在移动
    
    // 重写移动速度属性
    public override float moveSpeed
    {
        get { return 5.0f; } // 立方体的移动速度
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomMovementRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // 基本的Update逻辑可以留空，因为我们使用协程来处理移动
    }
    
    // 随机移动协程
    private IEnumerator RandomMovementRoutine()
    {
        while (true)
        {
            if (!isMoving && canMove)
            {
                isMoving = true;
                
                // 生成随机方向 (XY平面)
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    0 // 在XY平面移动，Z保持不变
                );
                
                // 调用基类的移动方法，移动2秒
                Move(randomDirection, 2.0f);
                
                // 等待移动完成
                yield return new WaitForSeconds(2.0f);
                isMoving = false;
                
                // 移动完成后立即开始下一次移动，不再等待
            }
            else
            {
                yield return null;
            }
        }
    }
}
