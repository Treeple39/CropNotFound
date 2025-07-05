using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个命名空间来管理场景

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



    [Tooltip("结算场景")]
    public string EndSceneName = "EndScene";
    public string endBGM = "";


    [Tooltip("抽卡场景")]
    public string DrawCardsName = "DrawCards";
    public string drawCardsBGM = "";


    [Tooltip("Thanks")]

    public string Thanks = "Thank";
    public string thankBGM = "";


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
        Debug.Log("GameManager: 返回主菜单...");
        SceneManager.LoadScene(mainMenuSceneName);
        DataManager.Instance.SetHasSeenOpeningAnimation(true);
        PlayBGM(mainMenuBGM);
    }

    /// <summary>
    /// 从主菜单开始剧情
    /// </summary>
    public void StartStory()
    {
        Debug.Log("GameManager: 开始加载开场动画场景...");
        SceneManager.LoadScene(openSceneName);

        PlayBGM(storyBGM);
    }


    /// <summary>
    /// 从开场剧情结束进入对话剧情
    /// </summary>
    public void StartLog()
    {
        Debug.Log("GameManager: 主菜单结束，开始加载对话场景...");
        SceneManager.LoadScene(storySceneName);

        PlayBGM(storyBGM);
    }



    /// <summary>
    /// 从对话剧情进入关卡
    /// </summary>
    public void StartLevel()
    {
        Debug.Log("GameManager: 对话场景结束，开始加载关卡场景...");
        SceneManager.LoadScene(levelSceneName);
        playSequenceBGM(levelsequenceBeginBGM, levelsequenceBodyBGM, levelsequenceConnectBGM);
    }




    public void GoToEndScene()
    {
        Debug.Log("GameManager: 前往结算..");
        SceneManager.LoadScene(EndSceneName);
        PlayBGM(endBGM);
    }

    public void GoToCardScene()
    {
        Debug.Log("GameManager: 前往抽卡..");
        SceneManager.LoadScene(DrawCardsName);
        PlayBGM(drawCardsBGM);
    }
    public void GoTOThanks()
    {
        Debug.Log("GameManager: 前往致谢..");
        SceneManager.LoadScene(Thanks);
        Score.ResetScore();
        PlayBGM("");
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
}