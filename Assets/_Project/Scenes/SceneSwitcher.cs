using UnityEngine;
using UnityEngine.Playables;
using System.Collections; // 引入协程所需的命名空间

[RequireComponent(typeof(PlayableDirector))]
public class OpeningAnimationManager : MonoBehaviour
{
    private PlayableDirector timelineDirector;

    private void Awake()
    {
        // 获取Timeline组件
        timelineDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        if (DataManager.Instance.HasSeenOpeningAnimation())
        {
            timelineDirector.Stop();
            // 启动一个协程，在延迟后跳转场景
            StartCoroutine(AutomaticSkipAfterDelay());
        }
        else
        {
            timelineDirector.stopped += OnTimelineFinished;
            timelineDirector.Play();
        }
    }

    /// <summary>
    /// 协程：在指定的延迟后跳转到主菜单。
    /// 仅在玩家已经看过开场动画时被调用。
    /// </summary>
    private IEnumerator AutomaticSkipAfterDelay()
    {
        // 等待4秒
        yield return new WaitForSeconds(5f);

        // 跳转到主菜单
        GoToNextScene();
    }

    /// <summary>
    /// 当Timeline完整播放结束时，由系统自动调用。
    /// 仅在玩家第一次观看动画时被触发。
    /// </summary>
    private void OnTimelineFinished(PlayableDirector _)
    {
        // 标记为已观看
        DataManager.Instance.SetHasSeenOpeningAnimation(true);

        // 跳转到主菜单
        GoToNextScene();
    }

    /// <summary>
    /// 封装了跳转场景的逻辑，方便复用并进行安全检查。
    /// </summary>
    private void GoToNextScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("场景跳转失败：GameManager 实例未找到！");
        }
    }

    /// <summary>
    /// 在对象销毁时，清理事件绑定，防止内存泄漏。
    /// </summary>
    private void OnDestroy()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }
}