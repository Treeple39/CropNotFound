using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Resources;

public class DataManager : Singleton<DataManager>
{
    private string _jsonConfigPath = "Config/";

    private string _SOSavePath => Path.Combine(Application.persistentDataPath, "SOSaves/");

    public Dictionary<int, ItemDetails> ItemDetails { get; private set; } 

    [Header("��SOģ��")]
    [SerializeField] private InventoryBag_SO bagSO;

    private const string AnimationSavePath = "AnimationState.json";

    private InventoryBag_SO playerBag;

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
        return false; 
    }

    // ����"�ѿ�������"״̬
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
            Debug.Log($"����SO�浵Ŀ¼: {_SOSavePath}");
        }
    }

    public void Init()
    {
        EnsureDirectoriesExist();
        Application.quitting += SaveAllDynamicData;
        LoadAllStaticConfigs();  
        LoadOrCreateDynamicData();
    }

    private void LoadAllStaticConfigs()
    {
        TextAsset textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "ItemSettings");
        string json = textAsset.text;
        this.ItemDetails = JsonConvert.DeserializeObject<Dictionary<int, ItemDetails>>(json);

        Debug.Log($"���� {ItemDetails.Count} ����Ʒ����");
    }

    private void LoadOrCreateDynamicData()
    {
        string inventorySavePath = Path.Combine(_SOSavePath, "PlayerInventory.json");
        if (bagSO == null)
        {
            return;
        }

        // 2. ���ԴӴ浵����
        if (File.Exists(inventorySavePath))
        {
            string json = File.ReadAllText(inventorySavePath);
            playerBag = Instantiate(bagSO);
            JsonUtility.FromJsonOverwrite(json, playerBag);
        }
        else
        {
            // 3. �޴浵ʱ����¡ģ����Ϊ�´浵
            playerBag = Instantiate(bagSO);
        }
    }

    // === �浵���� ===
    public void SaveAllDynamicData()
    {
        if (!Directory.Exists(_SOSavePath))
            Directory.CreateDirectory(_SOSavePath);
        string inventoryJson = JsonUtility.ToJson(playerBag);
        File.WriteAllText(Path.Combine(_SOSavePath, "PlayerInventory.json"), inventoryJson);
    }

    // === ���߷��� ===
    public ItemDetails GetItemDetail(int itemId)
    {
        if (ItemDetails.TryGetValue(itemId, out var detail))
            return detail;
        return null;
    }
}