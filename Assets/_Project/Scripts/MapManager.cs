using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 地图管理器，负责在MainScene中根据静态变量 TargetRoomName 自动加载和卸载地图预制体。
/// </summary>
public class MapManager : MonoBehaviour
{
    // --- 单例模式实现 ---
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中查找实例
                _instance = FindObjectOfType<MapManager>();

                // 如果场景中没有，则创建一个新的GameObject并添加该脚本
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MapManager).Name);
                    _instance = singletonObject.AddComponent<MapManager>();
                }
            }
            return _instance;
        }
    }

    [Header("地图预制体")]
    public GameObject livingroom; // 将你的客厅预制体拖到这里
    public GameObject bathroom;   // 将你的浴室预制体拖到这里

    [Header("场景名称")]
    [SerializeField] private string _mainSceneName = "MainScene";

    /// <summary>
    /// 【核心】静态变量，用于跨场景指定要在MainScene中加载的房间名称。
    /// 其他脚本可以直接通过 MapManager.TargetRoomName = "livingroom"; 来设置。
    /// </summary>
    public static string TargetRoomName = "livingroom";

    private GameObject _currentRoomInstance; // 当前实例化的房间对象

    private void Awake()
    {
        // --- 保证全局只有一个实例 ---
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // 确保MapManager在场景切换时不被销毁

        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 在对象销毁时取消订阅，防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 当新场景加载完成时被调用
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果加载的是主场景，则根据静态变量自动加载地图
        if (scene.name == _mainSceneName)
        {
            LoadRoomFromStaticVariable();
        }
        // 如果离开主场景，则自动销毁当前地图
        else
        {
            DestroyCurrentRoom();
        }
    }

    /// <summary>
    /// 根据 TargetRoomName 的值加载并实例化对应的房间预制体
    /// </summary>
    private void LoadRoomFromStaticVariable()
    {
        if (string.IsNullOrEmpty(TargetRoomName))
        {
            Debug.LogWarning("MapManager: TargetRoomName 未设置，将不会加载任何地图。");
            return;
        }

        // 在加载新房间前，确保旧的已被销毁
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
                Debug.LogError($"MapManager: 无法识别的房间名称 '{TargetRoomName}'，请检查拼写或预制体设置。");
                break;
        }

        if (prefabToLoad != null)
        {
            _currentRoomInstance = Instantiate(prefabToLoad);
            Debug.Log($"成功在 {_mainSceneName} 中加载地图: {TargetRoomName}");
        }
        else
        {
            Debug.LogError($"MapManager: 名为 '{TargetRoomName}' 的地图预制体未在MapManager中设置！");
        }
    }

    /// <summary>
    /// 销毁当前场景中由MapManager创建的房间实例
    /// </summary>
    private void DestroyCurrentRoom()
    {
        if (_currentRoomInstance != null)
        {
            Destroy(_currentRoomInstance);
            _currentRoomInstance = null; // 置空引用，防止悬挂
        }
    }

    public void init()
    {

    }
}