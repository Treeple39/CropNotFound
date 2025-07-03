// DashCooldownUI.cs

using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

[RequireComponent(typeof(Image))]
public class UI_DashCooldown : MonoBehaviour
{
    // 在Inspector中，将包含PlayerMovement脚本的玩家对象拖到这里
    [SerializeField]
    private PlayerMovement playerMovement; 

    private Image cooldownImage;

    void Awake()
    {
        // 获取挂载此脚本对象上的Image组件
        cooldownImage = GetComponent<Image>();
    }

    void Update()
    {
        if (playerMovement != null)
        {
            // 从PlayerMovement脚本获取冷却进度 (0到1)
            float fill = playerMovement.DashCooldownProgress;
            
            // 将进度赋值给Image的fillAmount属性
            cooldownImage.fillAmount = fill;

            // (可选) 如果你希望冷却完成时UI消失，冷却开始时UI出现，可以取消下面的注释
            // cooldownImage.enabled = (fill < 1.0f); 
            // 如果有背景图，也需要一起控制
            // transform.parent.gameObject.SetActive(fill < 1.0f); 
        }
    }
}