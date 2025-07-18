using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIShop : MonoBehaviour
{
    [Header("按钮设置")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button goBackButton;
    
    [Header("货币显示")]
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [Header("物品展示区域")]
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemPrefab; 
    [SerializeField] private ScrollRect scrollRect;  
    
    [Header("物品数据")]
    [SerializeField] private List<Sprite> itemIcons;
    
    private void Start()
    {
        drawCardButton.onClick.AddListener(OnDrawCardButtonClick);
        goBackButton.onClick.AddListener(OnGoBackButtonClick);
        UpdateCoinsDisplay();
    }

    private void OnEnable()
    {
        ShopDataManager.Instance.RefreshCoins();
        UpdateCoinsDisplay();
    }

    private void UpdateCoinsDisplay()
    {
        coinsText.text = $"硬币: {ShopDataManager.Instance.GetCurrentCoins()}";
    }

    private void OnDrawCardButtonClick()
    {
        int itemId = ShopDataManager.Instance.DrawItem();
        
        if (itemId == -1)
        {
            Debug.Log("硬币不足，无法抽卡");

        }
        else
        {
            UpdateCoinsDisplay();
            ShowAcquiredItem(itemId);
        }
    }

    private void ShowAcquiredItem(int itemId)
    {
        GameObject newItem = Instantiate(itemPrefab, itemsContainer);

        ItemUI itemUI = newItem.GetComponent<ItemUI>();
        if (itemUI != null)
        {

            int iconIndex = itemId - 1000;
            if (iconIndex >= 0 && iconIndex < itemIcons.Count)
            {
                itemUI.Setup(itemIcons[iconIndex], $"物品{itemId}");
            }
        }
        
        Canvas.ForceUpdateCanvases();
        scrollRect.horizontalNormalizedPosition = 1f;
    }

    private void OnGoBackButtonClick()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        drawCardButton.onClick.RemoveListener(OnDrawCardButtonClick);
        goBackButton.onClick.RemoveListener(OnGoBackButtonClick);
    }
}