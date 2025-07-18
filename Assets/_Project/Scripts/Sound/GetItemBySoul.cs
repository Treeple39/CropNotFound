using Inventory;
using System.Collections; // 引入协程所需的命名空间
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 控制抽奖按钮的音效序列。
/// 依赖于 BaseSoundController。
/// </summary>
public class LotteryButtonSoundController : BaseSoundController
{
    [Header("抽奖音效设置")]
    [Tooltip("抽奖进行时播放的循环音效。")]
    [SerializeField] private AudioClip lotteryInProgressSound;

    [Tooltip("抽中道具后播放的音效，和添加道具。")]
    [SerializeField] private AudioClip itemAcquiredSound;
    [SerializeField] private itemUITipDatabase itemUITipDatabase;
    public Coroutine lotteryCoroutine; // 存储正在运行的协程

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 公共方法：启动抽奖流程。
    /// 这个方法应该被绑定到Unity按钮的 OnClick() 事件上。
    /// </summary>
    public void StartLotterySequence(int itemId)
    {
        if (lotteryCoroutine != null)
        {
            Debug.Log("抽奖流程已在进行中，请勿重复点击。");
            return;
        }

        lotteryCoroutine = StartCoroutine(PlayLotterySequenceCoroutine(itemId));
    }

    /// <summary>
    /// 协程：按顺序播放音效并最终显示UI。
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator PlayLotterySequenceCoroutine(int itemId)
    {

        // 播放抽奖过程音效
        if (lotteryInProgressSound != null)
        {
            PlaySound(lotteryInProgressSound);
            yield return new WaitForSeconds(lotteryInProgressSound.length);
        }
        else
        {
            Debug.LogWarning($"在 {gameObject.name} 上未设置 'Lottery In Progress Sound'。", this);
        }

        // 播放获得道具音效
        if (itemAcquiredSound != null)
        {

            PlaySound(itemAcquiredSound);
            ItemUIData itemGet = itemUITipDatabase.GetItemUIData(itemId);
            EventHandler.CallItemGet(itemGet, 10);
            InventoryManager.Instance.AddItem(itemId);
            yield return new WaitForSeconds(itemAcquiredSound.length);
        }
        else
        {
            Debug.LogWarning($"在 {gameObject.name} 上未设置 'Item Acquired Sound'。", this);
        }
        lotteryCoroutine = null;
    }
}