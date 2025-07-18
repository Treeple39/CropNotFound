using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    [Header("场景名称配置")]
    [Tooltip("开场动画场景的文件名")]
    public string openSceneName = "OpeningAnimation";
    public string openBGM = "";


    [Tooltip("主菜单场景的文件名")]
    public string mainMenuSceneName = "MainMenuScene";
    public string mainMenuBGM = "";


    [Tooltip("剧情场景的文件名")]
    public string storySceneName = "StoryScene";
    public string storyBGM = "";



    [Tooltip("主关卡场景的文件名")]
    public string levelSceneName = "LevelScene";
    public string levelsequenceBeginBGM = "";
    public string levelsequenceBodyBGM = "";
    public string levelsequenceConnectBGM = "";

    [Tooltip("抽道具场景")]
    public string SceneName = "DRAWITEM";
    public string sequenceBeginBGM = "";
    public string sequenceBodyBGM = "";
    public string sequenceConnectBGM = "";



    [Tooltip("结算场景")]
    public string EndSceneName = "EndScene";
    public string endBGM = "";


    [Tooltip("抽卡场景")]
    public string DrawCardsName = "DrawCards";
    public string drawCardsBGM = "";


    [Tooltip("Thanks")]

    public string Thanks = "Thank";
    public string thankBGM = "";

    [Tooltip("​​Archive​​")]

    public string Archive = "Archive";
    public string archiveBGM = "";

    private Stack<string> sceneHistory = new Stack<string>();


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        PlayBGM(mainMenuBGM);
    }

    /// <summary>
    /// 返回主菜单（用于将来的“退出到菜单”功能）
    /// </summary>
    public void GoToMainMenu()
    {
        UIManager.Instance.SetAllUIPanelsActive(false);
        Debug.Log("GameManager: 返回主菜单...");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        UIManager.Instance.FadeOut();
        LoadSceneWithHistory(mainMenuSceneName, mainMenuBGM);
        Score.ResetScore();
        DataManager.Instance.SetHasSeenOpeningAnimation(true);
    }

    /// <summary>
    /// 从主菜单开始剧情
    /// </summary>
    public void StartStory()
    {
        Debug.Log("GameManager: 开始加载开场动画场景...");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        UIManager.Instance.FadeOut();
        LoadSceneWithHistory(openSceneName, storyBGM);
    }


    /// <summary>
    /// 从开场剧情结束进入对话剧情
    /// </summary>
    public void StartLog()
    {
        Debug.Log("GameManager: 主菜单结束，开始加载对话场景...");
        LoadSceneWithHistory(storySceneName, storyBGM);
    }

    public void DrawItem()
    {
        Debug.Log("完蛋了");
        LoadSceneWithHistory(SceneName, storyBGM);
    }



    /// <summary>
    /// 从对话剧情进入关卡
    /// </summary>
    public void StartLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != levelSceneName)
        {
            sceneHistory.Push(currentScene);
            Debug.Log("Scene pushed to history: " + currentScene);
        }
        RoomData selectedRoom = DataManager.Instance.GetRandomRoom();

        if (selectedRoom != null)
        {
            // 将房间名赋值给 MapManager 的静态变量
            MapManager.TargetRoomName = selectedRoom.roomName;
            Debug.Log($"[MainMenu] 已从JSON配置随机选择地图: {selectedRoom.roomName}");
        }
        else
        {
            // 出错时的备用方案
            MapManager.TargetRoomName = "livingroom";
            Debug.LogError("[MainMenu] 无法从DataManager获取随机房间, 使用默认 'livingroom'.");
        }

        ScoreDetector.Instance._lastItemCount = 0;
        Debug.Log("GameManager: 对话场景结束，开始加载关卡场景...");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        UIManager.Instance.FadeOut();
        UIManager.Instance.SetAllUIPanelsActive(true);
        SceneManager.LoadScene(levelSceneName);
        ShopDataManager.Instance.RefreshHasAdd();
        playSequenceBGM(levelsequenceBeginBGM, levelsequenceBodyBGM, levelsequenceConnectBGM);
    }

    public void GoToEndScene()
    {
        Debug.Log("GameManager: 前往结算..");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        UIManager.Instance.SetAllUIPanelsActive(false);
        SceneManager.LoadScene(EndSceneName);
        //UIManager.Instance.UILevelUpPanel.OpenTab();
        PlayBGM(endBGM);
    }

    public void GoToCardScene()
    {
        Debug.Log("GameManager: 前往抽卡..");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        UIManager.Instance.SetAllUIPanelsActive(false);
        SceneManager.LoadScene(DrawCardsName);
        PlayBGM(drawCardsBGM);
    }
    public void GoToThanks()
    {
        Debug.Log("GameManager: 前往致谢..");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        SceneManager.LoadScene(Thanks);
        PlayBGM("");
    }

    public void GoToArchive()
    {
        Debug.Log("GameManager: 前往图鉴..");
        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        LoadSceneWithHistory(Archive, archiveBGM);
    }

    public void SkipOpeningAnimation()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }


    private static void PlayBGM(string bgmPath)
    {
        if (AudioManager.S != null)
        {
            AudioManager.S.StopBGM();
            AudioClip audioClip = ResourceManager.Load<AudioClip>(bgmPath);
            if (audioClip != null)
            {
                AudioManager.S.PlayBGM(ResourceManager.Load<AudioClip>(bgmPath));
            }
            else
            {
                Debug.LogError("could not find the " + bgmPath + " BGM");
            }
        }
    }
    private static void playSequenceBGM(string beginBGMPath, string bodyBGMPath, string connectBGMPath)
    {
        if (AudioManager.S != null)
        {
            AudioManager.S.StopBGM();
            SequencedBgm sequencedBgm = new SequencedBgm
            {
                introClip = ResourceManager.Load<AudioClip>(beginBGMPath),
                themeClip = ResourceManager.Load<AudioClip>(bodyBGMPath),
                bridgeClip = ResourceManager.Load<AudioClip>(connectBGMPath),
            };
            if (sequencedBgm.introClip == null)
            {
                Debug.LogError("could not find the " + beginBGMPath + " BGM");
            }
            if (sequencedBgm.themeClip == null)
            {
                Debug.LogError("could not find the " + bodyBGMPath + " BGM");
            }
            if (sequencedBgm.bridgeClip == null)
            {
                Debug.LogError("could not find the " + connectBGMPath + " BGM");
            }
            AudioManager.S.PlaySequencedBGM(sequencedBgm);
        }
    }
    public void LoadSceneWithHistory(string newSceneName, string bgmPath = "")
    {
        string currentScene = SceneManager.GetActiveScene().name;
        sceneHistory.Push(currentScene);
        Debug.Log("Scene pushed to history: " + currentScene);

        UIManager.Instance.UIMessagePanel.ForceClosePanel();
        SceneManager.LoadScene(newSceneName);
        PlayBGM(bgmPath);
    }

    public void GoBackToPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory.Pop();
            Debug.Log("Returning to previous scene: " + previousScene);
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene in history!");
        }
    }
}