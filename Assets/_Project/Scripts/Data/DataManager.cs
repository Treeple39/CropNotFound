using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Resources;

public class DataManager : Singleton<DataManager>
{
    // Static config path (JSON files in Resources)
    private string _jsonConfigPath = "Config/";

    // Persistent SO save path (for runtime data)
    private string _SOSavePath => Path.Combine(Application.persistentDataPath, "SOSaves/");

    // Static data loaded from JSON
    public Dictionary<int, ItemDetails> ItemDetails { get; private set; } // Item settings
    public Dictionary<int, EnemyDetails> EnemyDetails { get; private set; } // Tech level requirements
    public Dictionary<int, SkillDetails> SkillDetails { get; private set; } // Tech level requirements
    public Dictionary<int, TechLevelEventData> TechLevelEventDatas { get; private set; } // Tech level unlock triggers
    public Dictionary<int, TechLevelDetails> TechLevelDetails { get; private set; } // Tech level requirements
    public List<RoomData> RoomDetails { get; private set; }  //≥°æ∞…Ë÷√

    // Runtime SO data (e.g. inventory, tech level, unlock progress)
    [Header("SO Model")]
    [SerializeField] private InventoryBag_SO bagSO;
    [SerializeField] private TechLevel_SO techLevelSO;
    [SerializeField] private TechUnlockProgess_SO techUnlockProgessSO;

    // Animation opening flag (whether opening animation has been seen)
    private const string AnimationSavePath = "AnimationState.json";

    [Header("SO Temp Data")]
    public InventoryBag_SO playerBag;
    public TechLevel_SO archiveTechLevel;
    public TechUnlockProgess_SO techUnlockProgess;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSeenOpeningAnimation()
    {
        string path = Path.Combine(_SOSavePath, AnimationSavePath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<bool>(json);
        }
        return false; // Default: not seen
    }

    // Save the opening animation watched state
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
            Debug.Log($"Created SO save directory: {_SOSavePath}");
        }
    }

    public void Init()
    {
        EnsureDirectoriesExist();
        LoadAllStaticConfigs();
        LoadOrCreateDynamicData();
    }

    private void OnApplicationQuit()
    {
        SaveAllDynamicData();
    }

    // === Load static data from JSON configs ===
    private void LoadAllStaticConfigs()
    {
        TextAsset textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "ItemSettings");
        string json = textAsset.text;
        this.ItemDetails = JsonConvert.DeserializeObject<Dictionary<int, ItemDetails>>(json);

        textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "EnemySettings");
        json = textAsset.text;
        this.EnemyDetails = JsonConvert.DeserializeObject<Dictionary<int, EnemyDetails>>(json);

        textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "SkillSettings");
        json = textAsset.text;
        this.SkillDetails = JsonConvert.DeserializeObject<Dictionary<int, SkillDetails>>(json);

        textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "TechLevelEventSettings");
        json = textAsset.text;
        this.TechLevelEventDatas = JsonConvert.DeserializeObject<Dictionary<int, TechLevelEventData>>(json);

        textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "TechLevelSettings");
        json = textAsset.text;
        this.TechLevelDetails = JsonConvert.DeserializeObject<Dictionary<int, TechLevelDetails>>(json);
        
        textAsset = Resources.Load<TextAsset>(_jsonConfigPath + "RoomSettings");
        json = textAsset.text;
        this.RoomDetails = JsonConvert.DeserializeObject<List<RoomData>>(json);
        Debug.Log($"Loaded {ItemDetails.Count} item configs.");
    }

    // === Load or create dynamic data (SO runtime state) ===
    private void LoadOrCreateDynamicData()
    {
        string inventorySavePath = Path.Combine(_SOSavePath, "PlayerInventory.json");
        string techLevelSavePath = Path.Combine(_SOSavePath, "ArchiveTechLevel.json");
        string techUnlockSavePath = Path.Combine(_SOSavePath, "TechUnlockProgess.json");

        playerBag = JsonOverwriteSO(inventorySavePath, bagSO);
        archiveTechLevel = JsonOverwriteSO(techLevelSavePath, techLevelSO);
        techUnlockProgess = JsonOverwriteSO(techUnlockSavePath, techUnlockProgessSO);
    }

    private T JsonOverwriteSO<T>(string jsonPath, T model) where T : ScriptableObject
    {
        T instance = Instantiate(model);
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            JsonUtility.FromJsonOverwrite(json, instance);
            Debug.Log($"Loaded JSON into {typeof(T)}: {json}");
        }
        else
        {
            Debug.Log($"Created new {typeof(T)} instance (no JSON found)");
        }
        return instance;
    }


    // === Save all runtime data ===
    public void SaveAllDynamicData()
    {
        if (!Directory.Exists(_SOSavePath))
            Directory.CreateDirectory(_SOSavePath);

        // Save data to Application.persistentDataPath
        string inventoryJson = JsonUtility.ToJson(playerBag);
        File.WriteAllText(Path.Combine(_SOSavePath, "PlayerInventory.json"), inventoryJson);

        string techLevelJson = JsonUtility.ToJson(archiveTechLevel);
        File.WriteAllText(Path.Combine(_SOSavePath, "ArchiveTechLevel.json"), techLevelJson);

        string techUnlockJson = JsonUtility.ToJson(techUnlockProgess);
        File.WriteAllText(Path.Combine(_SOSavePath, "TechUnlockProgess.json"), techUnlockJson);

        Debug.Log($"[SaveAllDynamicData] Saving unlocked items: {string.Join(",", techUnlockProgess.unlockedItemIDs)}");
    }

    public void SaveDynamicData(ScriptableObject SO, string name)
    {
        if (!Directory.Exists(_SOSavePath))
            Directory.CreateDirectory(_SOSavePath);

        string Json = JsonUtility.ToJson(SO);
        File.WriteAllText(Path.Combine(_SOSavePath, name), Json);
    }

    // === Utility functions ===
    public ItemDetails GetItemDetail(int itemId)
    {
        if (ItemDetails.TryGetValue(itemId - 1000, out var detail))
            return detail;

        Debug.LogError($"Item ID {itemId} not found in config.");
        return null;
    }

    public EnemyDetails GetMonsterDetail(int itemId)
    {
        if (EnemyDetails.TryGetValue(itemId - 8000, out var detail))
            return detail;

        Debug.LogError($"Enemy ID {itemId} not found in config.");
        return null;
    }

    public SkillDetails GetSkillDetail(int itemId)
    {
        if (SkillDetails.TryGetValue(itemId - 6000, out var detail))
            return detail;

        Debug.LogError($"Skill ID {itemId} not found in config.");
        return null;
    }

    public RoomData GetRandomRoom()
    {
        if (RoomDetails == null || RoomDetails.Count == 0)
        {
            Debug.LogError("RoomDetails list is empty! Cannot select a random room. Make sure RoomSettings.json is loaded correctly.");
            return null;
        }

        int randomIndex = Random.Range(0, RoomDetails.Count);
        return RoomDetails[randomIndex];
    }

}