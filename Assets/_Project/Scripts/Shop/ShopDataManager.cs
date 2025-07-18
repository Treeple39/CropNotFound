using Inventory;
using UnityEngine;

public class ShopDataManager : Singleton<ShopDataManager>
{
    [SerializeField] private int _currentCoins;
    private bool _hasAddedScore = false;

    [SerializeField] private int _costPerDraw = 500;
    private readonly int[] _itemIds = { 1000, 1001, 1002 };
    [SerializeField] private LotteryButtonSoundController lotteryButtonSoundController;

    public void Init()
    {
        RefreshCoins();
    }
    public void RefreshHasAdd()
    {
        _hasAddedScore = false;
    }
    public void RefreshCoins()
    {
        if (DataManager.Instance == null)
        {
            return;
        }
        if (!_hasAddedScore)
        {
            _currentCoins = DataManager.Instance.playerCurrency.coins + Mathf.RoundToInt(Score.score);
            _hasAddedScore = true;
        }
        else
        {
            _currentCoins = DataManager.Instance.playerCurrency.coins;
        }
        _currentCoins = 1000;
        Score.ResetScore();
        DataManager.Instance.playerCurrency.coins = _currentCoins;
        DataManager.Instance.SaveAllDynamicData();
        Debug.Log($"当前商店硬币: {_currentCoins}");
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

        Debug.Log($"抽卡成功！获得物品ID: {chosenItemId}, 剩余硬币: {_currentCoins}");

        return chosenItemId;
    }

    public void ResetScoreAddition()
    {
        _hasAddedScore = false;
    }

    public int GetCurrentCoins() => _currentCoins;
    public int GetCostPerDraw() => _costPerDraw;
}