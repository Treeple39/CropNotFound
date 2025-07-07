using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEventManager : MonoBehaviour
{
    public static ItemEventManager Instance;

    [Header("UI References")]
    [SerializeField]public GameObject popupWindow;
    [SerializeField] public Image eventImage;
    [SerializeField] public Text messageText;

    [Header("Settings")]
    [SerializeField] public float speedBoostDuration = 5f;
    [SerializeField] public float speedBoostMultiplier = 2f;
    [SerializeField] public int soulAte = 2; //����������趨�Զ��ٸ���괥��һ��

    [Header("Item Settings")]
    [SerializeField] public List<Item> possibleItems; // ��ƷԤ�����б�������itemID��

    
    public List<ItemUIData> itemUIDataList; // ����itemID��ͼƬ/�ı��Ĺ���


    private Dictionary<int, ItemUIData> _itemUIDict; // ���ֵ���ٲ���
    private int _lastItemCount;


    private void TriggerRandomEvent()
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= .6f)       // 0.0-0.6 (60%)
            ShowRandomMessage();
        else if (randomValue < 0.9f&&randomValue>0.6f)  // 0.6-0.9 (30%)
            AddRandomItem();
        else                          // 0.9-1.0 (10%)
            BoostPlayerSpeed();
    }

    // �����ʾ��Ϣ��60%���ʣ�
    private void ShowRandomMessage()
    {
        if (itemUIDataList.Count == 0) return;

        // ���ѡһ����Ʒ��UI����
        var randomData = itemUIDataList[Random.Range(0, 5)];
        eventImage.sprite = randomData.messageImage;
        messageText.text = randomData.message;
        ShowPopup(3f);
    }

    // ��������Ʒ��30%���ʣ�   ��Ϊ����Item�ǱߵĽṹ��������ͽ������ˣ�
    private void AddRandomItem()
    {
        if (possibleItems.Count == 0) return;

        Item randomItemPrefab = possibleItems[Random.Range(0, possibleItems.Count)];
        Item newItem = Instantiate(randomItemPrefab);

        // �������InventoryManager.AddItem()
        InventoryManager.Instance.AddItem(0);

        // ����itemID��ʾ��ӦUI
        if (_itemUIDict.TryGetValue(newItem.itemID, out var itemData)||true)
        {
            eventImage.sprite = itemData.messageImage;
            messageText.text = $"���: {itemData.message}";
            ShowPopup(3f);
        }
    }

    // ������ң�10%���ʣ�
    private void BoostPlayerSpeed()
    {
        if (PlayerMovement.Instance == null) return;

        PlayerMovement.Instance.UpdateMaxVelocity(2f);
        var Data = itemUIDataList[7];
        eventImage.sprite = Data.messageImage;
        messageText.text = $"�ٶ����� {speedBoostMultiplier}��! ���� {speedBoostDuration}��,�尡��";
        ShowPopup(3f);
        Invoke(nameof(ResetPlayerSpeed), speedBoostDuration);
    }

    private void ShowPopup(float duration)
    {
        popupWindow.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(duration));
    }
    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // ����Time.timeScaleӰ��
        HidePopup();
    }

    private void ResetPlayerSpeed()
    {
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.UpdateMaxVelocity(0.5f);
        }
    }

    private void HidePopup() => popupWindow.SetActive(false);
}