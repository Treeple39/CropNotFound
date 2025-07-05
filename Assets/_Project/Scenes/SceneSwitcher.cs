using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector))]
public class OpeningAnimationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button skipButton; // 拖拽UI跳过按钮到Inspector

    private PlayableDirector timelineDirector;

    private void Awake()
    {
        // 获取Timeline组件
        timelineDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // 初始化跳过按钮状态
        skipButton.gameObject.SetActive(ShouldShowSkipButton());

        // 绑定Timeline结束事件
        timelineDirector.stopped += OnTimelineFinished;

        // 绑定跳过按钮事件
        skipButton.onClick.AddListener(SkipAnimation);
    }

    private void Update()
    {
        // 当跳过按钮可见时，检测任意按键跳过
        if (skipButton.gameObject.activeSelf && Input.anyKeyDown)
        {
            SkipAnimation();
        }
    }

    /// <summary>
    /// 判断是否应该显示跳过按钮
    /// </summary>
    private bool ShouldShowSkipButton()
    {
        return DataManager.Instance.HasSeenOpeningAnimation();
    }

    /// <summary>
    /// Timeline播放结束时自动调用
    /// </summary>
    private void OnTimelineFinished(PlayableDirector _)
    {
        // 首次播放时标记为已观看
        if (!DataManager.Instance.HasSeenOpeningAnimation())
        {
            DataManager.Instance.SetHasSeenOpeningAnimation(true);
        }

        // 确保GameManager存在后跳转
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("GameManager实例未找到！");
        }
    }

    /// <summary>
    /// 手动跳过动画
    /// </summary>
    private void SkipAnimation()
    {
        // 立即标记为已观看（即使未播放完）
        DataManager.Instance.SetHasSeenOpeningAnimation(true);

        // 跳转到主菜单
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("跳转失败：GameManager未初始化");
        }
    }

    private void OnDestroy()
    {
        // 清理事件绑定
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }
}