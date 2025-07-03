using UnityEngine;
using UnityEngine.Video; // 必须引用这个命名空间
using System.Collections; // 必须引用这个命名空间来使用协程

// 确保这个GameObject上有VideoPlayer组件
[RequireComponent(typeof(VideoPlayer))]
public class VideoController : MonoBehaviour
{
    // --- 引用设置 ---
    [Header("要控制的视频播放器")]
    private VideoPlayer videoPlayer;


    void Awake()
    {
        // 获取同一个GameObject上的VideoPlayer组件
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        // 游戏一开始就播放视频
        PlayVideoAndNotifyOnEnd();
    }

    /// <summary>
    /// 播放视频，并在结束后执行操作
    /// </summary>
    public void PlayVideoAndNotifyOnEnd()
    {
        // 检查VideoPlayer和GameManager是否存在
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer 或 GameManager 没有被指定！");
            return;
        }
        
        // 开始播放视频
        videoPlayer.Play();
        
        // 启动一个协程来监听播放结束
        StartCoroutine(CheckVideoEndCoroutine());
    }

    /// <summary>
    /// 监听视频播放结束的协程
    /// </summary>
    private IEnumerator CheckVideoEndCoroutine()
    {
        Debug.Log("视频开始播放，正在监听结束...");

        // 等待一小段时间，确保视频已经开始播放
        yield return new WaitForSeconds(0.5f);
        
        // 核心逻辑：当videoPlayer正在播放时，这个循环会一直暂停在下一帧
        while (videoPlayer.isPlaying)
        {
            yield return null; // 等待下一帧
        }
        
        // 当循环跳出时，就意味着视频播放结束了
        Debug.Log("视频播放结束！");

        GameManager.Instance.GoToMainMenu();
    }
}