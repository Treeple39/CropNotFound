using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个命名空间来管理场景

public class GameManager : MonoBehaviour
{
    // --- 单例模式实现 ---
    public static GameManager Instance { get; private set; }

    [Header("场景名称配置")]
    [Tooltip("剧情场景的文件名")]
    public string storySceneName = "StoryScene";
    [Tooltip("主关卡场景的文件名")]
    public string levelSceneName = "LevelScene";
    [Tooltip("主菜单场景的文件名")]
    public string mainMenuSceneName = "MainMenuScene";

    private void Awake()
    {
        // 这是实现单例模式和跨场景保留的核心代码
        if (Instance == null)
        {
            // 如果Instance不存在，则将此实例设为单例
            Instance = this;
            // 并告诉Unity在加载新场景时不要销毁这个GameObject
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果场景中已存在一个GameManager，则销毁这个新的（重复的）实例
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 从主菜单开始剧情
    /// </summary>
    public void StartStory()
    {
        Debug.Log("GameManager: 开始加载剧情场景...");
        SceneManager.LoadScene(storySceneName);
    }

    /// <summary>
    /// 从剧情结束进入关卡
    /// </summary>
    public void StartLevel()
    {
        Debug.Log("GameManager: 剧情结束，开始加载关卡场景...");
        SceneManager.LoadScene(levelSceneName);
    }

    /// <summary>
    /// 返回主菜单（用于将来的“退出到菜单”功能）
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("GameManager: 返回主菜单...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}