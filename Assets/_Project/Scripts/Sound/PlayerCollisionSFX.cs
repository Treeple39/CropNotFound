using UnityEngine;

public class PlayerCollisionSFX : BaseSoundController 
{
    [Header("玩家专属碰撞音效")]
    [Tooltip("被'Slippers'(鞋子)类型敌人撞击时播放的音效")]
    [SerializeField] private AudioClip hitBySlippersSound;

    [Tooltip("碰到'Bear'(熊)类型敌人时播放的音效")]
    [SerializeField] private AudioClip touchBearSound;

    [Tooltip("碰到其他任何可交互/障碍物时播放的通用音效")]
    [SerializeField] private AudioClip genericHitSound;

    // OnCollisionEnter2D 会在玩家的碰撞体与其他碰撞体开始接触时被调用
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 尝试从碰撞对象获取 Enemy 脚本
        Enemy enemyComponent = collision.gameObject.GetComponent<Enemy>();

        if (enemyComponent != null)
        {
            // 根据敌人身上的移动脚本来区分敌人类型
            if (collision.gameObject.GetComponent<SlippersMovement>() != null)
            {
                PlaySound(hitBySlippersSound); // 调用基类的播放方法
            }
            else if (collision.gameObject.GetComponent<DollMovement>() != null)
            {
                PlaySound(touchBearSound);
            }
            else
            {
                PlaySound(genericHitSound);
            }
        }
        else
        {
            // 撞到非敌人对象，比如墙壁
            if (collision.gameObject.CompareTag("Objects")|| collision.gameObject.CompareTag("Border"))
            {
                PlaySound(genericHitSound);
            }
        }
    }
}