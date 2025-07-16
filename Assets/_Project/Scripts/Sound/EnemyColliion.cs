using UnityEngine;

public class MonsterCollisionSFX : BaseSoundController // 同样继承自基类
{
    [Header("怪物专属碰撞音效")]
    [Tooltip("怪物撞到墙壁或其他障碍物时播放的音效")]
    [SerializeField] private AudioClip bumpSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 核心逻辑：检查撞到的物体**不是**玩家
        if (!collision.gameObject.CompareTag("Player"))
        {
            // 为了避免怪物之间互相碰撞也发出声音，可以进一步限定只在撞墙时响
            if (collision.gameObject.CompareTag("Wall"))
            {
                PlaySound(bumpSound); // 调用基类的播放方法
            }
        }
    }
}