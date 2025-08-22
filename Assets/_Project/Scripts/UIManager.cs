using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Inventory;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] public UIMessagePanel UIMessagePanel;
    [SerializeField] public UISystemMessage UISystemMessagePanel;
    [SerializeField] public UITechLevel TechLevelPanel;
    [SerializeField] public UILevelUpPanel UILevelUpPanel;
    [SerializeField] public InventoryUI BagUIPanel;


    [Header("set active use")]
    [SerializeField] public GameObject BagPanel;
    [SerializeField] public GameObject ScorePanel;

    [SerializeField] public GameObject JoyStick;
    [SerializeField] public GameObject ArchivePanel;
    [SerializeField] public GameObject SettingPanel;
    [SerializeField] public GameObject ShopPanel;
    [SerializeField] private GameObject MainLevelShopButton;

    private bool _isSubscribed = false;
    [HideInInspector] public bool joystickCanBeActive;

    [Header("fade use")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("UFX use")]
    [HideInInspector] public List<FxImagePos> RewardFxImagePosSet = new List<FxImagePos>();
    [SerializeField] private GameObject flyShopGoodsPrefab;
    [SerializeField] private Transform bagUITransform;


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
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            StartCoroutine(DelayedRefresh(10));
        }
    }

    private IEnumerator DelayedRefresh(int frames)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

    }

    public void RefreshJoystick()
    {
        JoyStick.SetActive(joystickCanBeActive);
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
        if (ShopPanel == null || BagPanel == null)
        {
            Debug.LogError($"缺失引用: ShopPanel={ShopPanel}, BagPanel={BagPanel}");
            return;
        }

        if (isActive && (DataManager.Instance?.playerCurrency == null))
        {
            Debug.LogWarning("玩家货币数据未就绪，延迟打开商店");
            StartCoroutine(DelayedOpenShop());
            return;
        }

        try
        {
            ShopPanel.SetActive(isActive);

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

    public IEnumerator GoodsFlytoBag()
    {
        if (RewardFxImagePosSet.Count > 0)
        {
            Vector2 targetPosition = bagUITransform.position;

            for (int i = 0; i < RewardFxImagePosSet.Count; i++)
            {
                if (!RewardFxImagePosSet[i].canfly)
                    continue;
                GameObject flygoods = Instantiate(flyShopGoodsPrefab, RewardFxImagePosSet[i].pos, Quaternion.identity, UIManager.Instance.transform);
                flygoods.GetComponent<FlyShopGoods>().Setup(RewardFxImagePosSet[i]);
                StartCoroutine(FlyToTarget(flygoods, targetPosition));
            }

            BagPanel.SetActive(true);
            BagUIPanel.bagUIAnim.SetBool("opened", true);
            RewardFxImagePosSet.Clear();
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator FlyToTarget(GameObject item, Vector2 targetPos)
    {
        yield return new WaitForSeconds(1.8f);

        float duration = 0.8f;
        float elapsed = 0f;
        Vector2 startPos = item.transform.position;

        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);
            item.transform.position = Vector2.Lerp(startPos, targetPos, t);
            item.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t * 0.5f);
            yield return null;
        }
        yield return null;
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance._runtimeInventory.itemList);

        Destroy(item);
    }
}
