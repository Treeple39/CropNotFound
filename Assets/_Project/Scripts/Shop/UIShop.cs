using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class UIShop : Singleton<UIShop>
{
    [Header("按钮设置")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button goBackButton;

    [Header("货币显示")]
    [SerializeField] private Text coinsText;

    [Header("物品展示区域")]
    [SerializeField] private GameObject itemsContainer;
    [SerializeField] private ItemUI[] ItemUIs;
    [SerializeField] private int NowItemUISite = 0;
    [SerializeField] private int maxDisplayItems = 18;

    [Header("物品数据")]
    [SerializeField] private List<Sprite> itemIcons;

    [Header("默认状态")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Start()
    {
        drawCardButton.onClick.AddListener(OnDrawCardButtonClick);
        goBackButton.onClick.AddListener(OnGoBackButtonClick);
        UpdateCoinsDisplay();
        ShopDataManager.Instance.RefreshCoins();
    }

    private void OnEnable()
    {
        if (itemsContainer != null)
        {
            ItemUIs = itemsContainer.GetComponentsInChildren<ItemUI>(true);
        }
        else
        {
            Debug.LogError("ItemsContainer is not assigned!");
            return;
        }

        // 添加长度验证
        if (ItemUIs == null || ItemUIs.Length == 0)
        {
            Debug.LogError("No ItemUI components found");
            return;
        }

        maxDisplayItems = ItemUIs.Length;
        ResetAllItems();
        UpdateCoinsDisplay();
    }
    private void OnDisable()
    {
        // 在UIShop中添加
        drawCardButton.onClick.RemoveAllListeners();
        goBackButton.onClick.RemoveAllListeners();
    }


    private void ResetAllItems()
    {
        NowItemUISite = 0;

        foreach (var itemUI in ItemUIs)
        {
            if (itemUI != null)
            {
                itemUI.Setup(defaultIcon, defaultColor);
            }
        }
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
        if (NowItemUISite >= ItemUIs.Length)
        {
            Debug.LogError($"Index out of range: {NowItemUISite}/{ItemUIs.Length}");
            return;
        }

        int iconIndex = itemId - 1000;
        if (iconIndex < 0 || iconIndex >= itemIcons.Count)
        {
            Debug.LogError($"Invalid icon index: {iconIndex} for item {itemId}");
        }
        else
        {
            ItemUIs[iconIndex].Setup(itemIcons[iconIndex], Color.white);
        }

        NowItemUISite++;
    }

    private void OnGoBackButtonClick()
    {
        UIManager.Instance.SetShopPanelActive(false);
    }

}