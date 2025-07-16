using UnityEngine;

public class SpeedValueAddBuff : BaseBuff
{
    // 这个Buff增加的移速数值，可以在Inspector里配置
    [SerializeField] private float speedIncreaseAmount = 2.0f;

    private void Awake()
    {
        buffName = "移速增加";
    }

    public override void ApplyBuff(GameObject target)
    {
        // 假设玩家身上有一个名为 PlayerMovement 的脚本控制移动
        PlayerMovement movementController = target.GetComponent<PlayerMovement>();

        if (movementController != null)
        {
            // 直接修改底层数据
            movementController.maxSpeed += speedIncreaseAmount;
            Debug.Log($"应用Buff: '{buffName}'，基础移速增加 {speedIncreaseAmount}！");
        }
        else
        {
            Debug.LogWarning($"在目标'{target.name}'上没有找到 PlayerMovement 脚本，无法应用速度Buff！");
        }
    }
}