using Inventory;
using System.Collections; // ����Э������������ռ�
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// ���Ƴ齱��ť����Ч���С�
/// ������ BaseSoundController��
/// </summary>
public class LotteryButtonSoundController : BaseSoundController
{
    [Header("�齱��Ч����")]
    [Tooltip("�齱����ʱ���ŵ�ѭ����Ч��")]
    [SerializeField] private AudioClip lotteryInProgressSound;

    [Tooltip("���е��ߺ󲥷ŵ���Ч������ӵ��ߡ�")]
    [SerializeField] private AudioClip itemAcquiredSound;
    [SerializeField] private itemUITipDatabase itemUITipDatabase;
    public Coroutine lotteryCoroutine; // �洢�������е�Э��

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// ���������������齱���̡�
    /// �������Ӧ�ñ��󶨵�Unity��ť�� OnClick() �¼��ϡ�
    /// </summary>
    public void StartLotterySequence(int itemId)
    {
        if (lotteryCoroutine != null)
        {
            Debug.Log("�齱�������ڽ����У������ظ������");
            return;
        }

        lotteryCoroutine = StartCoroutine(PlayLotterySequenceCoroutine(itemId));
    }

    /// <summary>
    /// Э�̣���˳�򲥷���Ч��������ʾUI��
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator PlayLotterySequenceCoroutine(int itemId)
    {

        // ���ų齱������Ч
        if (lotteryInProgressSound != null)
        {
            PlaySound(lotteryInProgressSound);
            yield return new WaitForSeconds(lotteryInProgressSound.length);
        }
        else
        {
            Debug.LogWarning($"�� {gameObject.name} ��δ���� 'Lottery In Progress Sound'��", this);
        }

        // ���Ż�õ�����Ч
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
            Debug.LogWarning($"�� {gameObject.name} ��δ���� 'Item Acquired Sound'��", this);
        }
        lotteryCoroutine = null;
    }
}