using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Resources;

public class DataManager : Singleton<DataManager>
{
    // 静态配置路径（JSON）
    private string _jsonConfigPath = "Config/";

    // 动态数据保存路径（SO）
    private string _SOSavePath => Path.Combine(Application.persistentDataPath, "SOSaves/");

    // 静态配置数据（JSON加载）
    public Dictionary<int, ItemDetails> ItemDetails { get; private set; } // 物品详情表

    // 动态数据（SO引用 + 持久化）runtimeInventory
    [Header("类SO模板")]
    [SerializeField] private InventoryBag_SO bagSO;

    //是否看过开场动画
    private const string AnimationSavePath = "AnimationState.json";

    private InventoryBag_SO playerBag;

    public bool HasSeenOpeningAnimation()
    {
        string path = Path.Combine(_SOSavePath, AnimationSavePath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<bool>(json);
        }
        return false; // 默认没看过
    }

    // 设置"已看过动画"状态
    public void SetHasSeenOpeningAnimation(bool hasSeen)
    {
        string path = Path.Combine(_SOSavePath, AnimationSavePath);
        string json = JsonConvert.SerializeObject(hasSeen);
        File.WriteAllText(path, json);
    }

    private void EnsureDirectoriesExist()
    {
        if (!Directory.Exists(_SOSavePath))
        {
            Directory.CreateDirectory(_SOSavePath);
            Debug.Log($"创建SO存档目录: {_SOSavePath}");
        }
    }

    public void Init()
    {
        EnsureDirectoriesExist();
        Application.quitting += SaveAllDynamicData;
        LoadAllStaticConfigs();  //加载静态JSON配置
        LoadOrCreateDynamicData(); //初始化动态SO数据
    }

    // === 静态配置（JSON） ===
    private void LoadAllStaticConfigs()
    {
        // 加载物品配置表
        TextAsset textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "ItemSettings");
        string json = textAsset.text;
        this.ItemDetails = JsonConvert.DeserializeObject<Dictionary<int, ItemDetails>>(json);

        Debug.Log($"加载 {ItemDetails.Count} 条物品配置");
    }

    // === 动态数据（SO） ===
    private void LoadOrCreateDynamicData()
    {
        // 尝试从存档加载背包SO
        string inventorySavePath = Path.Combine(_SOSavePath, "PlayerInventory.json");
        // 1. 确保模板SO存在
        if (bagSO == null)
        {
            Debug.LogError("未分配默认背包SO模板！");
            return;
        }

        // 2. 尝试从存档加载
        if (File.Exists(inventorySavePath))
        {
            string json = File.ReadAllText(inventorySavePath);
            // 克隆模板SO以避免污染原始资产
            playerBag = Instantiate(bagSO);
            JsonUtility.FromJsonOverwrite(json, playerBag);
            Debug.Log("背包从存档加载成功");
        }
        else
        {
            // 3. 无存档时，克隆模板作为新存档
            playerBag = Instantiate(bagSO);
            Debug.Log("创建新背包（基于模板）");
        }
    }

    // === 存档管理 ===
    public void SaveAllDynamicData()
    {
        if (!Directory.Exists(_SOSavePath))
            Directory.CreateDirectory(_SOSavePath);

        //序列化当前背包状态,不保存模板SO）
        string inventoryJson = JsonUtility.ToJson(playerBag);
        File.WriteAllText(Path.Combine(_SOSavePath, "PlayerInventory.json"), inventoryJson);
        Debug.Log("动态数据已保存");
    }

    // === 工具方法 ===
    public ItemDetails GetItemDetail(int itemId)
    {
        if (ItemDetails.TryGetValue(itemId, out var detail))
            return detail;

        Debug.LogError($"物品ID {itemId + 1000} 不存在于配置表中！");
        return null;
    }
}