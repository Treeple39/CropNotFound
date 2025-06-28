using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemGenerator : MonoBehaviour
{
    // 预制体数组
    public GameObject[] prefabs;

    // 全局参数
    public int ItemTotalCount = 30;    // 物体总数
    public int ChairCount = 5;         // 椅子总数
    public int DollCount = 5;          // 玩偶总数
    public int BottleCount = 5;        // 奶瓶总数
    public int PillowCount = 5;        // 枕头总数
    public int BookCount = 5;          // 书籍总数
    public int SlippersCount = 5;      // 拖鞋总数
    public float ItemMinDistance = 0.5f; // 物体最小间距
    public float MovableItemRatio = 0.5f; // 可移动物体占物体总数的比例

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
    public Vector3 mapMin = new Vector3(-10f, -10f, 0f);
    public Vector3 mapMax = new Vector3(10f, 10f, 0f);

    // 用于追踪可移动物体的数量
    private int movableItemCount = 0;

    private void Start()
    {
        // 加载预制体
        LoadPrefabs();
        
        // 生成物品
        StartCoroutine(GenerateItems());
    }

    private void LoadPrefabs()
    {
        // 从Resources文件夹加载预制体
        // 注意：预制体应该放在Resources文件夹下
        prefabs = Resources.LoadAll<GameObject>("_Project/Prefabs");
        
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

        // 确保物品总数不超过各类物品之和
        int totalItemsFromCategories = remainingItems.Values.Sum();
        if (ItemTotalCount > totalItemsFromCategories)
        {
            ItemTotalCount = totalItemsFromCategories;
            Debug.LogWarning("物体总数已调整为各类物品总和: " + ItemTotalCount);
        }

        // 计算应该有多少物体是可移动的
        int targetMovableItems = Mathf.RoundToInt(ItemTotalCount * MovableItemRatio);
        movableItemCount = 0; // 初始化可移动物体计数

        while (generatedCount < ItemTotalCount)
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
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(position, pos) < ItemMinDistance)
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
                GameObject item = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                spawnedPositions.Add(position);
                
                // 计算当前可移动物体的理想数量
                int idealMovableCount = Mathf.RoundToInt((currentCount + 1) * MovableItemRatio);
                
                // 决定这个物体是否可移动
                bool shouldBeMovable;
                
                if (movableItemCount < targetMovableItems && currentCount + 1 == ItemTotalCount)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
