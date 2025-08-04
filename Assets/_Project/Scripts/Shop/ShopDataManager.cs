using Inventory;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopDataManager : Singleton<ShopDataManager>
{
    [SerializeField] private int _currentCoins;


    [SerializeField] private int _costPerDraw = 500;
    private readonly int[] _itemIds = { 1000, 1001, 1002 };
    [SerializeField] private LotteryButtonSoundController lotteryButtonSoundController;

    public void Init()
    {
        RefreshCoins();
    }

    public void RefreshCoins()
    {
        if (DataManager.Instance == null)
        {
            return;
        }

        _currentCoins = DataManager.Instance.playerCurrency.coins;
        Debug.Log($"当前商店硬币: {_currentCoins}");
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

    public int DrawItem()
    {
        if (_currentCoins < _costPerDraw)
        {
            Debug.LogWarning("硬币不足！");
            return -1;
        }
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager未初始化！");
            return -1;
        }

        _currentCoins -= _costPerDraw;
        DataManager.Instance.playerCurrency.coins = _currentCoins;
        DataManager.Instance.SaveAllDynamicData();

        int randomIndex = Random.Range(0, _itemIds.Length);
        int chosenItemId = _itemIds[randomIndex];

        lotteryButtonSoundController.StartLotterySequence(chosenItemId);

        InventoryManager.Instance.AddItem(chosenItemId);
        DataManager.Instance.SaveAllDynamicData();

        Debug.Log($"抽卡成功！获得物品ID: {chosenItemId}, 剩余硬币: {_currentCoins}");

        return chosenItemId;
    }



    public int GetCurrentCoins() => _currentCoins;
    public int GetCostPerDraw() => _costPerDraw;
}