using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;

public class UIShop : Singleton<UIShop>
{
    [Header("按钮设置")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button goBackButton;

    [Header("货币显示")]
    [SerializeField] private Text coinsText;

    [Header("物品展示区域")]
    [SerializeField] private GameObject itemsContainer;
    [SerializeField] private GameObject shopGoodsPrefab;
    [SerializeField] private ItemUI[] ItemUIs;
    [SerializeField] private int NowItemUISite = 0;
    [SerializeField] private int maxDisplayItems = 18;

    [Header("物品数据")]
    [SerializeField] private List<Sprite> itemIcons;

    [Header("稀有度配置")]
    [SerializeField] private RarityDatabase rarityDatabase;

    [Header("默认状态")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private GameObject UFXPrefab;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        UpdateCoinsDisplay();
        int coins = ShopDataManager.Instance.RefreshCoins();
        drawCardButton.onClick.AddListener(OnDrawCardButtonClick);
        goBackButton.onClick.AddListener(OnGoBackButtonClick);
        if (itemsContainer != null)
        {
            int goodsNum = coins / ShopDataManager.Instance.costPerDraw;
            goodsNum = goodsNum > 15 ? 15 : goodsNum;
            List<ItemUI> items = new();

            for (int i = 0; i < goodsNum; i++)
            {
                items.Add(Instantiate(shopGoodsPrefab, itemsContainer.transform).GetComponentInChildren<ItemUI>(true));
            }
            
            ItemUIs = items.ToArray();
        }
        else
        {
            Debug.LogError("ItemsContainer is not assigned!");
            return;
        }


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
        UIManager.Instance.StartCoroutine(UIManager.Instance.GoodsFlytoBag());
        drawCardButton.onClick.RemoveAllListeners();
        goBackButton.onClick.RemoveAllListeners();
        ItemUIs = null;
        Config.RemoveAllChildren(itemsContainer);

    }

    private void ResetAllItems()
    {
        NowItemUISite = 0;

        foreach (var itemUI in ItemUIs)
        {
            if (itemUI != null)
            {
                itemUI.Setup(defaultIcon, defaultColor, 0, rarityDatabase.GetRarityData(Rarity.Common));
            }
        }
    }

    private void UpdateCoinsDisplay()
    {
        coinsText.text = $"硬币: {ShopDataManager.Instance.GetCurrentCoins()}";
    }

    private void OnDrawCardButtonClick()
    {
        ShopItem shopItem = ShopDataManager.Instance.DrawItem();

        if (shopItem == null)
        {
            Debug.Log("硬币不足，无法抽卡");
        }
        else
        {
            UpdateCoinsDisplay();
            ShowAcquiredItem(shopItem);
        }
    }

    private void ShowAcquiredItem(ShopItem shopItem)
    {
        if (NowItemUISite >= ItemUIs.Length)
        {
            Debug.LogError($"Index out of range: {NowItemUISite}/{ItemUIs.Length}");
            return;
        }

        int iconIndex = shopItem.itemId - 1000;
        if (iconIndex < 0 || iconIndex >= itemIcons.Count)
        {
            Debug.LogError($"Invalid icon index: {iconIndex} for item {shopItem.itemId}");
        }
        else
        {
            ItemUIs[NowItemUISite].Setup
            (
                itemIcons[iconIndex], 
                Color.white, 
                shopItem.amount,
                rarityDatabase.GetRarityData((Rarity)System.Enum.Parse(typeof(Rarity), DataManager.Instance.GetItemDetail(shopItem.itemId).itemRarity)),
                DataManager.Instance.GetItemDetail(shopItem.itemId).Name,
                true
            );
            Instantiate(UFXPrefab, ItemUIs[NowItemUISite].transform);
        }

        NowItemUISite++;
    }

    private void OnGoBackButtonClick()
    {
        UIManager.Instance.SetShopPanelActive(false);
    }

}