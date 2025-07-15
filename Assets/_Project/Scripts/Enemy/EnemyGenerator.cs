using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyGenerator : MonoBehaviour
{
    // 单例实例
    public static EnemyGenerator Instance { get; private set; }

    // 预制体数组
    public GameObject[] prefabs;

    // 全局参数
    public int ItemTotalCount = 10;    // 物品生成总数
    public float ItemMinDistance = 0.5f; // 物体最小间距

    // 持续生成相关参数
    public float continuousSpawnInterval = 1.0f; // 持续生成间隔(秒)
    private float spawnTimer = 0f;

    // 物品类型枚举
    private enum ItemType
    {
        Chair,
        Doll,
        Bottle,
        Pillow,
        Book,
        Slippers
    }

    // 记录已生成物品的位置
    public List<Transform> spawnedTransforms = new List<Transform>();

    // 地图边界
    public Vector3 mapMin = new Vector3(-40f, -40f, 0f);
    public Vector3 mapMax = new Vector3(30f, 10f, 0f);

    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 加载预制体
        LoadPrefabs();

        // 生成初始物品
        StartCoroutine(GenerateItems());
    }

    private void LoadPrefabs()
    {
        // 从Resources文件夹加载预制体
        // 注意：预制体应该放在Resources文件夹下
        prefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("未找到预制体，请确保预制体路径正确!");
        }
    }

    private IEnumerator GenerateItems()
    {
        int generatedCount = 0;
        
        Debug.Log("物体总数: " + ItemTotalCount);

        while (generatedCount < ItemTotalCount)
        {
            // 根据概率选择物品类型
            ItemType selectedType = SelectItemTypeByProbability();

            // 尝试生成物品
            if (TryGenerateItem(selectedType))
            {
                generatedCount++;

                // 每生成几个物品暂停一帧，避免卡顿
                if (generatedCount % 5 == 0)
                    yield return null;
            }
            else
            {
                // 如果无法生成，暂停一帧再试
                yield return null;
            }
        }

        Debug.Log($"物品生成完成，共生成 {generatedCount} 个物品");
    }

    private bool TryGenerateItem(ItemType itemType)
    {
        // 最大尝试次数，防止无限循环
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // 随机生成位置
            float x = Random.Range(mapMin.x, mapMax.x);
            float y = Random.Range(mapMin.y, mapMax.y);
            Vector3 position = new Vector3(x, y, 0);

            // 检查是否与其他物品有最小间距
            bool tooClose = false;
            foreach (Transform transform in spawnedTransforms)
            {
                if (transform == null) continue; // 跳过已销毁的物体
                if (Vector3.Distance(position, transform.position) < ItemMinDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
            {
                attempts++;
                continue;
            }

            // 生成物品
            GameObject prefab = GetPrefabByType(itemType);
            if (prefab != null)
            {
                GameObject item = Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));
                spawnedTransforms.Add(item.transform);

                // 获取并设置BaseMovement组件 - 所有物体都是可移动的
                BaseMovement movement = item.GetComponent<BaseMovement>();
                if (movement != null)
                {
                    movement.canMove = true;
                }
                else
                {
                    Debug.LogWarning($"物体 {item.name} 没有BaseMovement组件");
                }

                return true;
            }
            else
            {
                Debug.LogError($"未找到类型为 {itemType} 的预制体");
                return false;
            }
        }

        Debug.LogWarning($"无法为 {itemType} 找到合适的生成位置，已尝试 {maxAttempts} 次");
        return false;
    }

    private GameObject GetPrefabByType(ItemType itemType)
    {
        // 根据物品类型获取对应预制体
        // 这里假设预制体的名称包含物品类型名称
        string typeName = itemType.ToString();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name.Contains(typeName))
                return prefab;
        }

        // 如果找不到，返回第一个预制体(或随机一个)
        if (prefabs != null && prefabs.Length > 0)
            return prefabs[Random.Range(0, prefabs.Length)];

        return null;
    }

    // 根据概率计算器提供的数值选择要生成的物品类型
    private ItemType SelectItemTypeByProbability()
    {
        // 获取各种类型的概率权重
        float chairWeight = EnemyProbabilitCalculator.CalculateChairProbability();
        float dollWeight = EnemyProbabilitCalculator.CalculateDollProbability();
        float bottleWeight = EnemyProbabilitCalculator.CalculateBottleProbability();
        float pillowWeight = EnemyProbabilitCalculator.CalculatePillowProbability();
        float bookWeight = EnemyProbabilitCalculator.CalculateBookProbability();
        float slippersWeight = EnemyProbabilitCalculator.CalculateSlippersProbability();

        // 计算总权重
        float totalWeight = chairWeight + dollWeight + bottleWeight + pillowWeight + bookWeight + slippersWeight;

        // 随机值
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // 根据权重选择物品类型
        cumulativeWeight += chairWeight;
        if (randomValue <= cumulativeWeight) return ItemType.Chair;

        cumulativeWeight += dollWeight;
        if (randomValue <= cumulativeWeight) return ItemType.Doll;

        cumulativeWeight += bottleWeight;
        if (randomValue <= cumulativeWeight) return ItemType.Bottle;

        cumulativeWeight += pillowWeight;
        if (randomValue <= cumulativeWeight) return ItemType.Pillow;

        cumulativeWeight += bookWeight;
        if (randomValue <= cumulativeWeight) return ItemType.Book;

        return ItemType.Slippers;
    }

    // 生成单个持续物体
    private void SpawnContinuousItem()
    {
        // 选择物体类型
        ItemType selectedType = SelectItemTypeByProbability();

        // 尝试生成物体
        TryGenerateItem(selectedType);
    }

    // Update is called once per frame
    void Update()
    {
        // 处理持续生成物体的计时
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= continuousSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnContinuousItem();
        }
    }

    // 提供公共方法让其他脚本请求生成特定物品
    public bool RequestItemSpawn(string itemTypeName)
    {
        if (System.Enum.TryParse<ItemType>(itemTypeName, out ItemType itemType))
        {
            return TryGenerateItem(itemType);
        }

        Debug.LogError($"无效的物品类型名称: {itemTypeName}");
        return false;
    }
}
