using Inventory;
using UnityEngine;

public class ItemEat : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] float detectRange = 3f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private GameObject addItemEffectPrefab; // 拾取特效预制体
    public Item item;

    [Header("Coin Info")]
    public AudioClip coinGet;

    private bool isDestroyed = false;
    private bool canAttract = false;

    void Start()
    {
        // 初始时不能吸附，等待速度衰减完成
        canAttract = false;
    }

    void Update()
    {
        // 如果已经销毁或还不能吸附，则跳过
        if (isDestroyed || !canAttract) return;

        // 检测玩家并移动
        if (IsPlayerNearby())
        {
            MoveTowardsPlayer();
        }
    }

    // 由RandomVelocityDecay调用来启用吸附功能
    public void EnableAttraction()
    {
        canAttract = true;
    }

    public bool IsPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            return distance <= detectRange;
        }
        return false;
    }

    public void MoveTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance > minDistance)
        {
            // 平滑移动
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // 添加到库存
        InventoryManager.Instance.AddItem(item.itemID);

        // 播放音效
        AudioManager.S.PlayFX(coinGet, 0.5f, 1f);

        // 生成特效
        if (addItemEffectPrefab != null)
        {
            GameObject item = Instantiate(addItemEffectPrefab, transform.position, Quaternion.identity);
            item.GetComponentInChildren<SpriteRenderer>().sprite = this.item.spriteRenderer.sprite;
            item.GetComponent<Animator>().SetInteger("addMod", 2);
        }

        // 销毁物体
        Destroy(gameObject);
    }

    // 可视化检测范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}