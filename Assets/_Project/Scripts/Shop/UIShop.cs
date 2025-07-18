using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UIShop : MonoBehaviour
{
    [Header("按钮设置")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button goBackButton;
    
    [Header("货币显示")]
    [SerializeField] private TextMeshProUGUI coinsText;
    
    [Header("物品展示区域")]
    [SerializeField] private GameObject itemsContainer;
    [SerializeField] private ItemUI[] ItemUIs;
    [SerializeField] private int NowItemUISite = 0;
    [SerializeField] private int maxDisplayItems = 18;
    
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
        ItemUIs = GetComponentsInChildren<ItemUI>(itemsContainer);
        maxDisplayItems = ItemUIs.Count();
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
        // 检查是否超过最大数量
        if (NowItemUISite >= maxDisplayItems)
        {
            NowItemUISite = 0; // 回到第一个位置循环使用
        }

        ItemUI itemUI = ItemUIs[NowItemUISite];
        if (itemUI != null)
        {
            int iconIndex = itemId - 1000;
            if (iconIndex >= 0 && iconIndex < itemIcons.Count)
            {
                itemUI.Setup(itemIcons[iconIndex]);
            }
        }
        
        NowItemUISite++; // 移动到下一个位置
        Canvas.ForceUpdateCanvases();
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