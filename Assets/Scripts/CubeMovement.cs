using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : BaseMovement
{
    [Header("立方体移动设置")]
    public float moveDuration = 2.0f;     // 每次移动持续时间
    private float moveTimer = 0f;         // 移动计时器
    
    // 重写移动速度属性
    public override float moveSpeed
    {
        get { return 5.0f; } // 立方体的移动速度
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        // 启动第一次移动
        StartRandomMove();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 父类的Update会处理正在进行的移动
        base.Update();
        
        // 如果当前不在移动中，开始新的随机移动
        if (!isMoving)
        {
            StartRandomMove();
        }
    }
    
    // 开始随机移动
    private void StartRandomMove()
    {
        // 只有在允许移动的情况下才开始新的移动
        if (!canMove) return;
        
        // 生成随机方向 (XY平面)
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0 // 在XY平面移动，Z保持不变
        );
        
        // 调用基类的移动方法
        Move(randomDirection, moveDuration);
    }
}
