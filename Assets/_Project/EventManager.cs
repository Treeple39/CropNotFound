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
    [SerializeField] public int soulAte = 2; //在这里可以设定吃多少个灵魂触发一次

    [Header("Item Settings")]
    [SerializeField] public List<Item> possibleItems; // 物品预制体列表（需配置itemID）

    
    public List<ItemUIData> itemUIDataList; // 配置itemID与图片/文本的关联


    private Dictionary<int, ItemUIData> _itemUIDict; // 用字典快速查找
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

    // 随机显示消息（60%概率）
    private void ShowRandomMessage()
    {
        if (itemUIDataList.Count == 0) return;

        // 随机选一个物品的UI数据
        var randomData = itemUIDataList[Random.Range(0, 5)];
        eventImage.sprite = randomData.messageImage;
        messageText.text = randomData.message;
        ShowPopup(3f);
    }

    // 添加随机物品（30%概率）   因为理不清Item那边的结构所以这里就交给你了！
    private void AddRandomItem()
    {
        if (possibleItems.Count == 0) return;

        Item randomItemPrefab = possibleItems[Random.Range(0, possibleItems.Count)];
        Item newItem = Instantiate(randomItemPrefab);

        // 调用你的InventoryManager.AddItem()
        InventoryManager.Instance.AddItem(0);

        // 根据itemID显示对应UI
        if (_itemUIDict.TryGetValue(newItem.itemID, out var itemData)||true)
        {
            eventImage.sprite = itemData.messageImage;
            messageText.text = $"获得: {itemData.message}";
            ShowPopup(3f);
        }
    }

    // 加速玩家（10%概率）
    private void BoostPlayerSpeed()
    {
        if (PlayerMovement.Instance == null) return;

        PlayerMovement.Instance.UpdateMaxVelocity(2f);
        var Data = itemUIDataList[7];
        eventImage.sprite = Data.messageImage;
        messageText.text = $"速度提升 {speedBoostMultiplier}倍! 持续 {speedBoostDuration}秒,冲啊！";
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
        yield return new WaitForSecondsRealtime(delay); // 不受Time.timeScale影响
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