using UnityEngine;

public class RandomVelocityDecay : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float decayRate = 0.5f;

    private Rigidbody2D rb;
    private ItemEat itemEat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.drag = 0f;

        itemEat = GetComponent<ItemEat>();
        ApplyRandomVelocity();
    }

    void FixedUpdate()
    {
        if (rb.velocity != Vector2.zero)
        {
            Vector2 newVelocity = rb.velocity * (1 - decayRate * Time.fixedDeltaTime);

            if (newVelocity.magnitude < 0.01f)
            {
                newVelocity = Vector2.zero;
                OnMovementStopped();
            }

            rb.velocity = newVelocity;
        }
    }

    private void ApplyRandomVelocity()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        rb.velocity = randomDirection * randomSpeed;
    }

    private void OnMovementStopped()
    {
        // 移除物理组件
        Destroy(rb);

        // 启用ItemEat的吸附功能
        if (itemEat != null)
        {
            itemEat.EnableAttraction();
        }

        // 移除本脚本
        Destroy(this);
    }
}