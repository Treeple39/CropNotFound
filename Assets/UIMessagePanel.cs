using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessagePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public Image eventImage;
    [SerializeField] public Text messageText;
    [SerializeField] public GameObject messageContainer;
    [SerializeField] private Animator anim;
    [SerializeField] private List<ItemUIData> iTEM; 

    private itemUITipDatabase itemUIDataList;
    private Dictionary<int, ItemUIData> _itemUIDict;

    private void Start()
    {
        if (ScoreDetector.Instance != null)
        {
            ScoreDetector.Instance._lastItemCount = 0;
        }
    }

    public void InitMessages(itemUITipDatabase itemUIDataList)
    {
        this.itemUIDataList = itemUIDataList;

        _itemUIDict = new Dictionary<int, ItemUIData>();
        if (this.itemUIDataList != null && this.itemUIDataList.ItemUIDatas != null)
        {
            foreach (var data in this.itemUIDataList.ItemUIDatas)
            {
                _itemUIDict[data.messageID] = data;
            }
        }
    }

    private void OnEnable()
    {
        EventHandler.OnMessageShow += OnShowRandomMessage;
    }

    private void OnDisable()
    {
        EventHandler.OnMessageShow -= OnShowRandomMessage;
    }

    private void OnShowRandomMessage(ItemUIData itemUIData, float d)
    {
        if (_itemUIDict == null || _itemUIDict.Count == 0)
        {
            Debug.LogError("UIMessagePanel 的数据未初始化或为空，无法显示消息！");
            return;
        }

        anim.SetBool("close", false);
        messageContainer.SetActive(true);

        if (itemUIData != null)
        {
            eventImage.sprite = itemUIData.messageImage;
            messageText.text = itemUIData.message;
            StartCoroutine(CloseTab(5));
            return; 
        }
        List<ItemUIData> messages = new List<ItemUIData>(_itemUIDict.Values);
        ItemUIData randomMessage = messages[Random.Range(0, messages.Count)];

        if (randomMessage != null)
        {
            eventImage.sprite = randomMessage.messageImage;
            messageText.text = randomMessage.message;
            StartCoroutine(CloseTab(d)); // 使用事件传入的持续时间
        }
    }

    private IEnumerator CloseTab(float d)
    {
        yield return new WaitForSecondsRealtime(d);
        anim.SetBool("close", true);
    }

    public void ForceClosePanel()
    {
        anim.SetBool("close", true);
        messageContainer.SetActive(false);
        StopAllCoroutines();
    }
}