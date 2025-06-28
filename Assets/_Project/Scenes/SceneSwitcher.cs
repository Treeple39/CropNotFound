using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))] // 自动添加依赖组件
public class TimelineEndCallback : MonoBehaviour
{
    void Start()
    {
        // 获取Timeline的Director组件
        PlayableDirector director = GetComponent<PlayableDirector>();

        // 注册结束事件
        director.stopped += OnTimelineFinished;
    }

    // Timeline结束时自动触发
    private void OnTimelineFinished(PlayableDirector _)
    {
        // 直接调用目标方法
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartLog();
        }
        else
        {
            Debug.LogError("GameManager实例未找到！");
        }
    }
}