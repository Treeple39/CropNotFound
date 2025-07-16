using UnityEngine;

// 确保这个游戏对象上有关联的 Rigidbody2D 组件
[RequireComponent(typeof(Rigidbody2D))]
public class BookMovement : MonoBehaviour
{
    [Header("物理布朗运动设置")]
    [Tooltip("施加的随机力的基础大小。值越大，移动越剧烈。")]
    [SerializeField] private float randomForceMagnitude = 10f;

    [Tooltip("施加的随机旋转力的大小。值越大，旋转越疯狂。")]
    [SerializeField] private float torqueMagnitude = 5f;

    [Tooltip("随机移动时的空气阻力。值越大，移动越“粘滞”，不容易起速。")]
    [SerializeField] private float dragWhenIdle = 2f;

    [Tooltip("逃跑时的空气阻力。值要小一些，让它能跑得更快。")]
    [SerializeField] private float dragWhenFleeing = 0.5f;

    [Tooltip("逃跑时，远离玩家的力量的倍率。")]
    [SerializeField] private float fleeForceMultiplier = 2f;

    [Header("玩家检测")]
    [Tooltip("开始逃跑的玩家检测半径")]
    [SerializeField] private float playerDetectionRadius = 3f;

    [Tooltip("玩家对象的引用，如果为空会在开始时自动查找'Player'标签")]
    [SerializeField] private Transform player;

    // --- 内部变量 ---
    private Rigidbody2D rb;

    void Start()
    {
        // 获取刚体组件的引用
        rb = GetComponent<Rigidbody2D>();

        // 如果在Inspector中没有手动指定玩家，就尝试在场景中通过标签查找
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("场景中未找到带有 'Player' 标签的对象，书本将不会逃跑。", this.gameObject);
            }
        }
    }

    // FixedUpdate 是处理所有物理操作的最佳地方，它的调用频率是固定的
    void FixedUpdate()
    {
        // 首先，判断当前是否应该处于逃跑状态
        bool isFleeing = false;
        if (player != null)
        {
            // 计算书本与玩家之间的距离
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 如果玩家进入了检测范围，就设置逃跑状态为true
            if (distanceToPlayer <= playerDetectionRadius)
            {
                isFleeing = true;
            }
        }

        // --- 根据当前状态，施加不同的物理力和阻力 ---
        if (isFleeing)
        {
            // --- 逃跑状态 ---

            // 1. 设置较小的空气阻力，让它可以快速加速
            rb.drag = dragWhenFleeing;

            // 2. 计算一个远离玩家的基础方向
            Vector2 awayDirection = (transform.position - player.position).normalized;

            // 3. 在基础方向上叠加一个小的随机“抖动”，让逃跑路径不那么耿直
            Vector2 randomJitter = Random.insideUnitCircle * 0.3f;

            // 4. 计算最终的逃跑力，并施加到刚体上
            Vector2 fleeForce = (awayDirection + randomJitter).normalized * randomForceMagnitude * fleeForceMultiplier;
            rb.AddForce(fleeForce);
        }
        else
        {
            // --- 随机游荡状态 ---

            // 1. 设置较大的空气阻力，让它不会因为持续受力而速度无限增加，产生一种“粘滞感”
            rb.drag = dragWhenIdle;

            // 2. 生成一个完全随机的二维方向
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 3. 计算随机力并施加到刚体上
            Vector2 randomForce = randomDirection * randomForceMagnitude;
            rb.AddForce(randomForce);
        }

        // --- 无论处于何种状态，都给它施加一个随机的旋转力，让它不停地自旋，增加鬼畜感 ---
        float randomTorque = Random.Range(-1f, 1f) * torqueMagnitude;
        rb.AddTorque(randomTorque);
    }

    // 这个方法可以在编辑器中预览检测范围，非常便于调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}