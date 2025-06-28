using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个命名空间来管理场景

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("场景名称配置")]
    [Tooltip("剧情场景的文件名")]
    public string storySceneName = "StoryScene";
    [Tooltip("主关卡场景的文件名")]
    public string levelSceneName = "LevelScene";
    [Tooltip("主菜单场景的文件名")]
    public string mainMenuSceneName = "MainMenuScene";

    [Header("关卡音乐配置")]
    // 将你的序列BGM在这里公开，方便在Inspector中拖拽音频文件
    public SequencedBgm level1Bgm;

    private void Start()
    {
        if (AudioManager.S != null)
        {
            AudioManager.S.PlaySequencedBGM(level1Bgm);
        }
        else
        {
            Debug.Log("AudioManager.S = null");
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
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
        if (AudioManager.S != null)
        {
            AudioManager.S.PlaySequencedBGM(level1Bgm);
        }
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