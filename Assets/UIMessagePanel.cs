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

    private itemUITipDatabase itemUIDataList; // ����itemID��ͼƬ/�ı��Ĺ���
    private Dictionary<int, ItemUIData> _itemUIDict; // ���ֵ���ٲ���

    private void Start()
    {
        ///δ���Ž��ؿ���ʼ�� ����н���
        ScoreDetector.Instance._lastItemCount = 0;
    }
    
    public void InitMessages(itemUITipDatabase itemUIDataList)
    {
        this.itemUIDataList = itemUIDataList;
        // ��ʼ���ֵ�
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
