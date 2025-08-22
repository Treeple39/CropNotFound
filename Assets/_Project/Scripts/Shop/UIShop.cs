using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;

public class UIShop : MonoBehaviour
{
    [Header("按钮设置")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button drawAllCardButton;
    [SerializeField] private Button goBackButton;

    [Header("货币显示")]
    [SerializeField] private NumberCounterTMP coinsNumberCounter;

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

    [Header("默认配置")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private GameObject UFXPrefab;
    [SerializeField] private string flowOutMessage;
    [SerializeField] private string CoinsOutMessage;
    [SerializeField] private Sprite moneySprite;

    private void OnEnable()
    {
        UpdateCoinsDisplay();
        int coins = ShopDataManager.Instance.RefreshCoins();

        goBackButton.onClick.AddListener(OnGoBackButtonClick);
        NowItemUISite = 0;
        if (itemsContainer != null)
        {
            Config.RemoveAllChildren(itemsContainer);

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
        NowItemUISite = 0;
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
        coinsNumberCounter.SetValue(ShopDataManager.Instance.GetCurrentCoins());
    }

    public void OnDrawCardButtonClick()
    {
        if (NowItemUISite >= ItemUIs.Length)
        {
            EventHandler.CallSystemMessageShow(flowOutMessage,1.2f);
            return;
        }

        ShopItem shopItem = ShopDataManager.Instance.DrawItem();

        if (shopItem == null)
        {
            EventHandler.CallSystemMessageShow(CoinsOutMessage, 1.2f);
        }
        else
        {
            UpdateCoinsDisplay();
            ShowAcquiredItem(shopItem);
        }
    }

    public void OnDrawAllCardButtonClick()
    {
        if (NowItemUISite >= ItemUIs.Length)
        {
            EventHandler.CallSystemMessageShow(flowOutMessage, 1.2f);
            return;
        }

        int k = NowItemUISite;

        for (int i = 0; i < maxDisplayItems - k; i++)
        {
            ShopItem shopItem = ShopDataManager.Instance.DrawItem();

            if (shopItem == null)
            {
                EventHandler.CallSystemMessageShow(CoinsOutMessage, 1.2f);
            }
            else
            {
                ShowAcquiredItem(shopItem);
            }
        }
        UpdateCoinsDisplay();
    }

    private void ShowAcquiredItem(ShopItem shopItem)
    {
        if (NowItemUISite >= ItemUIs.Length)
        {
            Debug.LogError($"Index out of range: {NowItemUISite}/{ItemUIs.Length}");
            return;
        }

        if (shopItem.itemId == -1)
        {
            ItemUIs[NowItemUISite].Setup
            (
                moneySprite,
                Color.white,
                shopItem.amount,
                rarityDatabase.GetRarityData(Rarity.Legendary),
                "灵魂",
                true,
                false
            );
            Instantiate(UFXPrefab, ItemUIs[NowItemUISite].transform);
            NowItemUISite++;
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
        coinsNumberCounter.SetValue(0);
        UIManager.Instance.SetShopPanelActive(false);
    }

}