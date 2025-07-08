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

    private itemUITipDatabase itemUIDataList; // 配置itemID与图片/文本的关联
    private Dictionary<int, ItemUIData> _itemUIDict; // 用字典快速查找

    private void Start()
    {
        ///未来放进关卡初始化 里进行解耦
        ScoreDetector.Instance._lastItemCount = 0;
    }
    
    public void InitMessages(itemUITipDatabase itemUIDataList)
    {
        this.itemUIDataList = itemUIDataList;
        // 初始化字典
        _itemUIDict = new Dictionary<int, ItemUIData>();
        foreach (var data in this.itemUIDataList.ItemUIDatas)
        {
            _itemUIDict[data.messageID] = data;
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
        if (itemUIDataList.ItemUIDatas.Count == 0)
        {
            return;
        }
        anim.SetBool("close", false);
        messageContainer.SetActive(true);
        if(itemUIData.messageID == -1)
        {
            eventImage.sprite = itemUIData.messageImage;
            messageText.text = itemUIData.message;
            CloseTab(d);
            return;
        }

        ItemUIData randomMessage;
        if(_itemUIDict.TryGetValue(Random.Range(0, _itemUIDict.Count), out randomMessage))
        {
            eventImage.sprite = randomMessage.messageImage;
            messageText.text = randomMessage.message;
            StartCoroutine(CloseTab(d));
        }
    }

    private IEnumerator CloseTab(float d)
    {
        yield return new WaitForSecondsRealtime(d);
        anim.SetBool("close", true);
    }

}
