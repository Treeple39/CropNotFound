using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Resources;

public class DataManager : Singleton<DataManager>
{
    // ��̬����·����JSON��
    private string _jsonConfigPath = "Config/";

    // ��̬���ݱ���·����SO��
    private string _SOSavePath => Path.Combine(Application.persistentDataPath, "SOSaves/");

    // ��̬�������ݣ�JSON���أ�
    public Dictionary<int, ItemDetails> ItemDetails { get; private set; } // ��Ʒ�����

    // ��̬���ݣ�SO���� + �־û���runtimeInventory
    [Header("��SOģ��")]
    [SerializeField] private InventoryBag_SO bagSO;

    //�Ƿ񿴹���������
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
        return false; // Ĭ��û����
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
        LoadAllStaticConfigs();  //���ؾ�̬JSON����
        LoadOrCreateDynamicData(); //��ʼ����̬SO����
    }

    // === ��̬���ã�JSON�� ===
    private void LoadAllStaticConfigs()
    {
        // ������Ʒ���ñ�
        TextAsset textAsset = ResourceManager.Load<TextAsset>(_jsonConfigPath + "ItemSettings");
        string json = textAsset.text;
        this.ItemDetails = JsonConvert.DeserializeObject<Dictionary<int, ItemDetails>>(json);

        Debug.Log($"���� {ItemDetails.Count} ����Ʒ����");
    }

    // === ��̬���ݣ�SO�� ===
    private void LoadOrCreateDynamicData()
    {
        // ���ԴӴ浵���ر���SO
        string inventorySavePath = Path.Combine(_SOSavePath, "PlayerInventory.json");
        // 1. ȷ��ģ��SO����
        if (bagSO == null)
        {
            Debug.LogError("δ����Ĭ�ϱ���SOģ�壡");
            return;
        }

        // 2. ���ԴӴ浵����
        if (File.Exists(inventorySavePath))
        {
            string json = File.ReadAllText(inventorySavePath);
            // ��¡ģ��SO�Ա�����Ⱦԭʼ�ʲ�
            playerBag = Instantiate(bagSO);
            JsonUtility.FromJsonOverwrite(json, playerBag);
            Debug.Log("�����Ӵ浵���سɹ�");
        }
        else
        {
            // 3. �޴浵ʱ����¡ģ����Ϊ�´浵
            playerBag = Instantiate(bagSO);
            Debug.Log("�����±���������ģ�壩");
        }
    }

    // === �浵���� ===
    public void SaveAllDynamicData()
    {
        if (!Directory.Exists(_SOSavePath))
            Directory.CreateDirectory(_SOSavePath);

        //���л���ǰ����״̬,������ģ��SO��
        string inventoryJson = JsonUtility.ToJson(playerBag);
        File.WriteAllText(Path.Combine(_SOSavePath, "PlayerInventory.json"), inventoryJson);
        Debug.Log("��̬�����ѱ���");
    }

    // === ���߷��� ===
    public ItemDetails GetItemDetail(int itemId)
    {
        if (ItemDetails.TryGetValue(itemId, out var detail))
            return detail;

        Debug.LogError($"��ƷID {itemId + 1000} �����������ñ��У�");
        return null;
    }
}