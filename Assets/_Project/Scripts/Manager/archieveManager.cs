using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArchiveManager : MonoBehaviour
{
    // ========== 单例核心 ==========
    private static ArchiveManager _instance;
    public static ArchiveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ArchiveManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ArchiveManager");
                    _instance = go.AddComponent<ArchiveManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // ========== 数据结构 ==========
    [System.Serializable]
    private class ArchiveData
    {
        public List<ArchiveItem> archiveItems;
    }

    [System.Serializable]
    public class ArchiveItem
    {
        public int ID;
        public string imagePath;
        public bool isActivated;
    }

    // ========== 路径配置 ==========
    private const string RESOURCES_JSON_PATH = "CharacterArchive";
    private string PERSISTENT_JSON_PATH => Path.Combine(Application.persistentDataPath, "CharacterArchive.json");

    // ========== 状态 ==========
    private Dictionary<int, ArchiveItem> _itemDict = new Dictionary<int, ArchiveItem>();
    private bool _isInitialized = false; // ✅ 是否已初始化过

    // ========== 生命周期 ==========
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeOnce(); // ✅ 只做一次初始化
    }

    private void OnEnable()
    {
        if (_isInitialized)
        {
            RefreshFromDisk(); // ✅ 每次激活刷新一次
        }
    }

    // ========== 初始化与刷新 ==========
    private void InitializeOnce()
    {
        if (_isInitialized) return;

        LoadOrInitializeArchive();
        _isInitialized = true;
    }

    public void RefreshFromDisk()
    {
        TryLoadFromPersistentPath(); // ⚠️ 不再 fallback、保存，只更新
        Debug.Log("ArchiveManager: 数据已刷新");
    }

    private void LoadOrInitializeArchive()
    {
        if (TryLoadFromPersistentPath()) return;
        if (TryLoadFromResources()) return;

        Debug.LogError("无法加载任何存档数据！将创建空存档");
        _itemDict = new Dictionary<int, ArchiveItem>();
    }

    private bool TryLoadFromPersistentPath()
    {
        if (!File.Exists(PERSISTENT_JSON_PATH)) return false;

        string json = File.ReadAllText(PERSISTENT_JSON_PATH);
        if (string.IsNullOrEmpty(json)) return false;

        try
        {
            ArchiveData data = JsonUtility.FromJson<ArchiveData>(json);
            if (data?.archiveItems != null)
            {
                LoadItems(data.archiveItems);
                Debug.Log($"从持久化路径加载成功: {PERSISTENT_JSON_PATH}");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"持久化存档损坏: {e.Message}");
        }
        return false;
    }

    private bool TryLoadFromResources()
    {
        TextAsset defaultJson = Resources.Load<TextAsset>(RESOURCES_JSON_PATH);
        if (defaultJson == null) return false;

        try
        {
            ArchiveData defaultData = JsonUtility.FromJson<ArchiveData>(defaultJson.text);
            if (defaultData?.archiveItems != null)
            {
                LoadItems(defaultData.archiveItems);
                SaveToJson(); // 初次生成存档
                Debug.Log($"从Resources加载默认数据: {RESOURCES_JSON_PATH}");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"默认JSON解析失败: {e.Message}");
        }
        return false;
    }

    private void LoadItems(List<ArchiveItem> items)
    {
        _itemDict = new Dictionary<int, ArchiveItem>();
        foreach (var item in items)
        {
            _itemDict[item.ID] = item;
        }
    }

    private void SaveToJson()
    {
        ArchiveData data = new ArchiveData
        {
            archiveItems = new List<ArchiveItem>(_itemDict.Values)
        };

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(PERSISTENT_JSON_PATH, json);
        Debug.Log($"存档已保存到: {PERSISTENT_JSON_PATH}\n{json}");
    }

    // ========== 对外接口 ==========
    public void UnlockByStoryKey(int storyKey)
    {
        int archiveId = StoryKeyToArchiveId(storyKey);
        if (_itemDict.TryGetValue(archiveId, out ArchiveItem item) && !item.isActivated)
        {

            item.isActivated = true;
            SaveToJson();
            Debug.Log($"解锁角色ID: {archiveId} (剧情Key: {storyKey})");
        }
        // else
        // {
        //     Debug.LogError("未能解锁" + "storyKey" + storyKey + "archiveId" + archiveId);

        // }
    }

    public bool IsUnlocked(int archiveId) =>
        _itemDict.TryGetValue(archiveId, out ArchiveItem item) && item.isActivated;

    public ArchiveItem GetItem(int archiveId) =>
        _itemDict.TryGetValue(archiveId, out ArchiveItem item) ? item : null;

    public List<ArchiveItem> GetAllItems() =>
        new List<ArchiveItem>(_itemDict.Values);

    private int StoryKeyToArchiveId(int storyKey) =>
        (storyKey - 7) / 3 + 1;
}