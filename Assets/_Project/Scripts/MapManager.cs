using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��ͼ��������������MainScene�и��ݾ�̬���� TargetRoomName �Զ����غ�ж�ص�ͼԤ���塣
/// </summary>
public class MapManager : MonoBehaviour
{
    // --- ����ģʽʵ�� ---
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // �����ڳ����в���ʵ��
                _instance = FindObjectOfType<MapManager>();

                // ���������û�У��򴴽�һ���µ�GameObject����Ӹýű�
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MapManager).Name);
                    _instance = singletonObject.AddComponent<MapManager>();
                }
            }
            return _instance;
        }
    }

    [Header("��ͼԤ����")]
    public GameObject livingroom; // ����Ŀ���Ԥ�����ϵ�����
    public GameObject bathroom;   // �����ԡ��Ԥ�����ϵ�����

    [Header("��������")]
    [SerializeField] private string _mainSceneName = "MainScene";

    /// <summary>
    /// �����ġ���̬���������ڿ糡��ָ��Ҫ��MainScene�м��صķ������ơ�
    /// �����ű�����ֱ��ͨ�� MapManager.TargetRoomName = "livingroom"; �����á�
    /// </summary>
    public static string TargetRoomName = "livingroom";

    private GameObject _currentRoomInstance; // ��ǰʵ�����ķ������

    private void Awake()
    {
        // --- ��֤ȫ��ֻ��һ��ʵ�� ---
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // ȷ��MapManager�ڳ����л�ʱ��������

        // ���ĳ��������¼�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // �ڶ�������ʱȡ�����ģ���ֹ�ڴ�й©
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// ���³����������ʱ������
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ������ص���������������ݾ�̬�����Զ����ص�ͼ
        if (scene.name == _mainSceneName)
        {
            LoadRoomFromStaticVariable();
        }
        // ����뿪�����������Զ����ٵ�ǰ��ͼ
        else
        {
            DestroyCurrentRoom();
        }
    }

    /// <summary>
    /// ���� TargetRoomName ��ֵ���ز�ʵ������Ӧ�ķ���Ԥ����
    /// </summary>
    private void LoadRoomFromStaticVariable()
    {
        if (string.IsNullOrEmpty(TargetRoomName))
        {
            Debug.LogWarning("MapManager: TargetRoomName δ���ã�����������κε�ͼ��");
            return;
        }

        // �ڼ����·���ǰ��ȷ���ɵ��ѱ�����
        DestroyCurrentRoom();

        GameObject prefabToLoad = null;
        switch (TargetRoomName.ToLower())
        {
            case "livingroom":
                prefabToLoad = livingroom;
                break;
            case "bathroom":
                prefabToLoad = bathroom;
                break;
            default:
                Debug.LogError($"MapManager: �޷�ʶ��ķ������� '{TargetRoomName}'������ƴд��Ԥ�������á�");
                break;
        }

        if (prefabToLoad != null)
        {
            _currentRoomInstance = Instantiate(prefabToLoad);
            Debug.Log($"�ɹ��� {_mainSceneName} �м��ص�ͼ: {TargetRoomName}");
        }
        else
        {
            Debug.LogError($"MapManager: ��Ϊ '{TargetRoomName}' �ĵ�ͼԤ����δ��MapManager�����ã�");
        }
    }

    /// <summary>
    /// ���ٵ�ǰ��������MapManager�����ķ���ʵ��
    /// </summary>
    private void DestroyCurrentRoom()
    {
        if (_currentRoomInstance != null)
        {
            Destroy(_currentRoomInstance);
            _currentRoomInstance = null; // �ÿ����ã���ֹ����
        }
    }

    public void init()
    {

    }
}