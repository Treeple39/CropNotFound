using Inventory;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopDataManager : Singleton<ShopDataManager>
{
    [SerializeField] private int _currentCoins;

    [Header("单抽金额")]
    public int costPerDraw = 500;
    private readonly int[] _itemIds = { 1000, 1001, 1002, -1, -1, -1 };
    [SerializeField] private LotteryButtonSoundController lotteryButtonSoundController;
    public void Init()
    {
        RefreshCoins();
    }

    public int RefreshCoins()
    {
        if (DataManager.Instance == null)
        {
            return -1;
        }

        _currentCoins = DataManager.Instance.playerCurrency.coins;
        Debug.Log($"当前商店硬币: {_currentCoins}");

        return _currentCoins;
    }
    public void AddCoins(int NeedToAddCoins)
    {
        if (DataManager.Instance == null)
        {
            return;
        }


        DataManager.Instance.playerCurrency.coins = DataManager.Instance.playerCurrency.coins + NeedToAddCoins;
        _currentCoins = DataManager.Instance.playerCurrency.coins;
        DataManager.Instance.SaveAllDynamicData();
        
        Debug.Log($"已将score加入coins: {NeedToAddCoins}");
    }

    public ShopItem DrawItem()
    {
        if (_currentCoins < costPerDraw)
        {
            Debug.LogWarning("硬币不足！");
            return null;
        }
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager未初始化！");
            return null;
        }

        _currentCoins -= costPerDraw;
        DataManager.Instance.playerCurrency.coins = _currentCoins;
        DataManager.Instance.SaveDynamicData(DataManager.Instance.playerCurrency, "PlayerCurrency.json");

        //道具抽卡
        int randomIndex = Random.Range(0, _itemIds.Length);
        int chosenItemId = _itemIds[randomIndex];

        //数量抽卡
        int randomAmount = 1;
        float rate = Random.value;
        if (rate < 0.03f)
            randomAmount = 5;
        else if (rate < 0.15f)
            randomAmount = 3;
        else if (rate < 0.32f)
            randomAmount = 2;

        if (chosenItemId == -1)
        {
            float rateMoney = Random.value;
            if (rateMoney < 0.05f)
                randomAmount = 2000;
            else if (rateMoney < 0.15f)
                randomAmount = 1000;
            else if (rateMoney < 0.3f)
                randomAmount = 500;
            else
                randomAmount = 200;

            AddCoins(randomAmount);

            Debug.Log($"抽卡成功！获得硬币: {randomAmount}！");
        }
        else
        {
            lotteryButtonSoundController.StartLotterySequence(chosenItemId);

            //每次抽卡都自动存
            InventoryManager.Instance.AddItemByAmount(chosenItemId, randomAmount);
            DataManager.Instance.SaveDynamicData(InventoryManager.Instance._runtimeInventory, "PlayerInventory.json");

            Debug.Log($"抽卡成功！获得物品ID: {chosenItemId}, 剩余硬币: {_currentCoins}");
        }

        ShopItem shopItem = new ShopItem();
        shopItem.itemId = chosenItemId;
        shopItem.amount = randomAmount;

        return shopItem;
    
    }

    public int GetCurrentCoins() => _currentCoins;
    public int GetCostPerDraw() => costPerDraw;
}