using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemGenerator : MonoBehaviour
{
    // 单例实例
    public static ItemGenerator Instance { get; private set; }

    // 预制体数组
    public GameObject[] prefabs;

    // 全局参数
    public int ChairCount;         // 椅子总数
    public int DollCount;          // 玩偶总数
    public int BottleCount;        // 奶瓶总数
    public int PillowCount;        // 枕头总数
    public int BookCount;          // 书籍总数
    public int SlippersCount;      // 拖鞋总数
    public float ItemMinDistance = 0.5f; // 物体最小间距
    public float MovableItemRatio = 0.5f; // 可移动物体占物体总数的比例

    // 难度等级和持续生成相关参数
    public int difficultyLevel = 1; // 难度等级，初始值为1
    public float continuousSpawnInterval = 2.0f; // 持续生成间隔(秒)
    public int maxDifficultyLevel = 4; // 最大难度等级

    // 物品生成概率表（按难度等级）
    [System.Serializable]
    public class SpawnProbability
    {
        public float[] chairProb = { 0.02f, 0.06f, 0.10f, 0.15f };
        public float[] dollProb = { 0.90f, 0.70f, 0.50f, 0.25f };
        public float[] bottleProb = { 0.02f, 0.06f, 0.10f, 0.15f };
        public float[] pillowProb = { 0.02f, 0.06f, 0.10f, 0.15f };
        public float[] bookProb = { 0.02f, 0.06f, 0.10f, 0.15f };
        public float[] slippersProb = { 0.02f, 0.06f, 0.10f, 0.15f };
    }

    public SpawnProbability spawnProbability = new SpawnProbability();

    // 用于获取当前分数
    private Score scoreComponent;
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
    private List<Vector3> spawnedPositions = new List<Vector3>();
    
    // 地图边界
    public Vector3 mapMin = new Vector3(-20f, -20f, 0f);
    public Vector3 mapMax = new Vector3(20f, 20f, 0f);

    // 用于追踪可移动物体的数量
    private int movableItemCount = 0;

    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 可选：如果需要在场景切换时保留
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

        // 获取分数组件
        scoreComponent = FindObjectOfType<Score>();
        if (scoreComponent == null)
        {
            Debug.LogWarning("未找到Score组件，难度调整可能无法正常工作");
        }

        // 生成初始物品
        StartCoroutine(GenerateItems());
    }

    private void LoadPrefabs()
    {
        // 从Resources文件夹加载预制体
        // 注意：预制体应该放在Resources文件夹下
        prefabs = Resources.LoadAll<GameObject>("Prefabs");

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("未找到预制体，请确保预制体路径正确!");
        }
    }

    private IEnumerator GenerateItems()
    {
        int generatedCount = 0;
        Dictionary<ItemType, int> remainingItems = new Dictionary<ItemType, int>
        {
            { ItemType.Chair, ChairCount },
            { ItemType.Doll, DollCount },
            { ItemType.Bottle, BottleCount },
            { ItemType.Pillow, PillowCount },
            { ItemType.Book, BookCount },
            { ItemType.Slippers, SlippersCount }
        };

        // 计算物品总数为各类物品总和
        int itemTotalCount = remainingItems.Values.Sum();
        Debug.Log("物体总数: " + itemTotalCount);

        // 计算应该有多少物体是可移动的
        int targetMovableItems = Mathf.RoundToInt(itemTotalCount * MovableItemRatio);
        movableItemCount = 0; // 初始化可移动物体计数

        while (generatedCount < itemTotalCount)
        {
            // 随机选择物品类型
            List<ItemType> availableTypes = remainingItems.Where(kv => kv.Value > 0).Select(kv => kv.Key).ToList();

            if (availableTypes.Count == 0)
                break;

            ItemType selectedType = availableTypes[Random.Range(0, availableTypes.Count)];

            // 尝试生成物品
            if (TryGenerateItem(selectedType, generatedCount, targetMovableItems))
            {
                remainingItems[selectedType]--;
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

        Debug.Log($"物品生成完成，共生成 {generatedCount} 个物品，其中 {movableItemCount} 个为可移动物体");
    }

    private bool TryGenerateItem(ItemType itemType, int currentCount, int targetMovableItems)
    {
        // 最大尝试次数，防止无限循环
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // 随机生成位置
            float x = Random.Range(mapMin.x, mapMax.x);
            float z = Random.Range(mapMin.z, mapMax.z);
            Vector3 position = new Vector3(x, 0, z);

            // 检查是否与其他物品有最小间距
            bool tooClose = false;
            foreach (Transform transform in spawnedTransforms)
            {
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
                spawnedPositions.Add(position);
                
                // 计算当前可移动物体的理想数量
                int idealMovableCount = Mathf.RoundToInt((currentCount + 1) * MovableItemRatio);

                // 决定这个物体是否可移动
                bool shouldBeMovable;

                if (movableItemCount < targetMovableItems && currentCount + 1 == targetMovableItems)
                {
                    // 如果是最后一个物体，且可移动物体不足，则一定要设为可移动
                    shouldBeMovable = true;
                }
                else if (movableItemCount >= targetMovableItems)
                {
                    // 如果已经达到目标可移动数量，则不可移动
                    shouldBeMovable = false;
                }
                else
                {
                    // 根据当前比例决定是否可移动
                    shouldBeMovable = movableItemCount < idealMovableCount;
                }

                // 获取并设置BaseMovement组件
                BaseMovement movement = item.GetComponent<BaseMovement>();
                if (movement != null)
                {
                    movement.canMove = shouldBeMovable;
                    if (shouldBeMovable)
                    {
                        movableItemCount++;
                    }
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

    // 根据当前难度选择要生成的物品类型
    private ItemType SelectItemTypeByProbability()
    {
        // 获取当前难度等级的概率数组索引（数组索引从0开始，难度从1开始）
        int index = Mathf.Clamp(difficultyLevel - 1, 0, maxDifficultyLevel - 1);

        // 计算总概率
        float totalProb = spawnProbability.chairProb[index] +
                          spawnProbability.dollProb[index] +
                          spawnProbability.bottleProb[index] +
                          spawnProbability.pillowProb[index] +
                          spawnProbability.bookProb[index] +
                          spawnProbability.slippersProb[index];

        // 随机值
        float randomValue = Random.Range(0f, totalProb);
        float cumulativeProb = 0f;

        // 根据概率选择物品类型
        cumulativeProb += spawnProbability.chairProb[index];
        if (randomValue <= cumulativeProb) return ItemType.Chair;

        cumulativeProb += spawnProbability.dollProb[index];
        if (randomValue <= cumulativeProb) return ItemType.Doll;

        cumulativeProb += spawnProbability.bottleProb[index];
        if (randomValue <= cumulativeProb) return ItemType.Bottle;

        cumulativeProb += spawnProbability.pillowProb[index];
        if (randomValue <= cumulativeProb) return ItemType.Pillow;

        cumulativeProb += spawnProbability.bookProb[index];
        if (randomValue <= cumulativeProb) return ItemType.Book;

        return ItemType.Slippers;
    }

    // 生成单个持续物体
    private void SpawnContinuousItem()
    {
        // 选择物体类型
        ItemType selectedType = SelectItemTypeByProbability();

        // 尝试生成物体
        TryGenerateContinuousItem(selectedType);
    }

    // 尝试生成持续物体
    private bool TryGenerateContinuousItem(ItemType itemType)
    {
        // 最大尝试次数，防止无限循环
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // 随机生成位置
            float x = Random.Range(mapMin.x, mapMax.x);
            float z = Random.Range(mapMin.z, mapMax.z);
            Vector3 position = new Vector3(x, 0, z);

            // 检查是否与其他物品有最小间距
            bool tooClose = false;
            foreach (Transform transform in spawnedTransforms)
            {
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
                spawnedPositions.Add(position);
                
                // 获取并设置BaseMovement组件 - 所有持续生成的物体都是可移动的
                BaseMovement movement = item.GetComponent<BaseMovement>();
                if (movement != null)
                {
                    movement.canMove = true;
                    movableItemCount++;
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

        Debug.LogWarning($"无法为持续生成的 {itemType} 找到合适的生成位置，已尝试 {maxAttempts} 次");
        return false;
    }

    // 更新难度等级
    private void UpdateDifficultyLevel()
    {
        if (scoreComponent != null)
        {
            // 根据当前分数计算难度等级
            float score = scoreComponent.getScore();
            int newDifficultyLevel = Mathf.Min(Mathf.CeilToInt(score / 2000f), maxDifficultyLevel);

            //     if (newDifficultyLevel != difficultyLevel)
            //     {
            //         difficultyLevel = newDifficultyLevel;
            //         Debug.Log($"难度等级更新为：{difficultyLevel}");
            //     }
            // }
        }

        // Update is called once per frame
        void Update()
        {
            // 更新难度等级
            UpdateDifficultyLevel();

            // 处理持续生成物体的计时
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= continuousSpawnInterval)
            {
                spawnTimer = 0f;
                SpawnContinuousItem();
            }
        }
    }

    // 提供公共方法让其他脚本请求生成特定物品
    public bool RequestItemSpawn(string itemTypeName)
    {
        if (System.Enum.TryParse<ItemType>(itemTypeName, out ItemType itemType))
        {
            return TryGenerateContinuousItem(itemType);
        }

        Debug.LogError($"无效的物品类型名称: {itemTypeName}");
        return false;
    }
}
