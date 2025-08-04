using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] public UIMessagePanel UIMessagePanel;
    [SerializeField] public UISystemMessage UISystemMessagePanel;
    [SerializeField] public UITechLevel TechLevelPanel;
    [SerializeField] public UILevelUpPanel UILevelUpPanel;


    [Header("set active use")]
    [SerializeField] public GameObject BagPanel;
    [SerializeField] public GameObject ScorePanel;

    [SerializeField] public GameObject JoyStick;
    [SerializeField] public GameObject ArchivePanel;
    [SerializeField] public GameObject SettingPanel;
    [SerializeField] public GameObject ShopPanel;
    [SerializeField] private GameObject ShopButton;
    private bool _isSubscribed = false;
    [HideInInspector] public bool joystickCanBeActive;

    [Header("fade use")]
    [SerializeField] private CanvasGroup fadePanel;

    private const int SHOP_UNLOCK_LEVEL = 10;
    private bool _isDataReady = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        if (!_isSubscribed)
        {
            DataManager.OnAllDataLoaded += OnDataLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            _isSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (_isSubscribed)
        {
            DataManager.OnAllDataLoaded -= OnDataLoaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _isSubscribed = false;
        }
    }

    private void OnDataLoaded()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            _isDataReady = true;
            RefreshShopButtonState();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            StartCoroutine(DelayedRefresh(10));
        }
        else
        {
            ShopButton.SetActive(false);
        }
    }

    private IEnumerator DelayedRefresh(int frames)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

        RefreshShopButtonState();
    }

    public void RefreshJoystick()
    {
        JoyStick.SetActive(joystickCanBeActive);
    }
    public void RefreshShopButtonState()
    {
        if (!_isDataReady || ShopButton == null)
            return;

        if (TechLevelManager.Instance != null && TechLevelManager.Instance.CurrentTechLevel >= SHOP_UNLOCK_LEVEL)
        {
            ShopButton.SetActive(true);
        }

    }

    public void FadeUIDuration(CanvasGroup UIPanel, float fadeStrength, float duration)
    {
        UIPanel.gameObject.SetActive(true);
        UIPanel.DOFade(fadeStrength, duration);
    }

    public void RaiseMessageUI()
    {
        UIMessagePanel.gameObject.SetActive(true);
    }

    public void InitMessageUI(itemUITipDatabase itemUITipDatabase)
    {
        UIMessagePanel.InitMessages(itemUITipDatabase);
    }

    public void ShowHighlight(GameObject panel)
    {
        //Init Shining Obj to Guide the Panel
    }

    public void SetAllUIPanelsActive(bool isActive)
    {
        if (UILevelUpPanel != null)
        {
            UILevelUpPanel.gameObject.SetActive(isActive);
        }

        if (BagPanel != null)
        {
            BagPanel.SetActive(isActive);
        }

        if (ScorePanel != null)
        {
            ScorePanel.SetActive(isActive);
        }
        if (TechLevelPanel != null)
        {
            TechLevelPanel.gameObject.SetActive(isActive);
        }
    }
    public void SetArchivePanelActive(bool isActive)
    {
        if (ArchivePanel != null)
        {
            ArchivePanel.SetActive(isActive);
            EventHandler.CallArchivePanelStateChanged(isActive);
        }
        else
        {
            Debug.LogError("ArchivePanel不存在");
        }
    }

    public void SetShopPanelActive(bool isActive)
    {
        // 层级1：检查基础对象
        if (ShopPanel == null || BagPanel == null)
        {
            Debug.LogError($"缺失引用: ShopPanel={ShopPanel}, BagPanel={BagPanel}");
            return;
        }

        // 层级2：检查关键数据
        if (isActive && (DataManager.Instance?.playerCurrency == null))
        {
            Debug.LogWarning("玩家货币数据未就绪，延迟打开商店");
            StartCoroutine(DelayedOpenShop());
            return;
        }

        // 层级3：安全操作
        try
        {
            ShopPanel.SetActive(isActive);
            BagPanel.SetActive(isActive);

            // 如果是打开操作，刷新数据
            if (isActive)
            {
                ShopDataManager.Instance?.RefreshCoins();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"商店面板操作失败: {e.Message}");
        }
    }
    private IEnumerator DelayedOpenShop()
    {
        yield return new WaitUntil(() => DataManager.Instance?.playerCurrency != null);
        ShopPanel.SetActive(true);
        BagPanel.SetActive(true);
    }

    public void FadeIn(bool _out = false, float duration = 1.0f)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = 0;
        if (_out)
        {
            fadePanel.DOFade(1, duration).OnComplete(() => {
                fadePanel.DOFade(0, duration);
                fadePanel.gameObject.SetActive(false);
            });
            return;
        }
        fadePanel.DOFade(1, duration).OnComplete(() =>
        {
            fadePanel.gameObject.SetActive(false);
        });
        }

    public void FadeOut(bool _in = false, float duration = 1.0f)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = 1;
        if (_in)
        {
            fadePanel.DOFade(0, duration).OnComplete(() => {
                fadePanel.DOFade(1, duration);
                fadePanel.gameObject.SetActive(false);
            });
            return;
        }
        fadePanel.DOFade(0, duration).OnComplete(() =>
        {
            fadePanel.gameObject.SetActive(false);
        });
        }
}
