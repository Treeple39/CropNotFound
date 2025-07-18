using Inventory;
using UnityEngine;

public class ShopDataManager : Singleton<ShopDataManager>
{
    private int _currentCoins;
    private bool _hasAddedScore = false;

    [SerializeField] private int _costPerDraw = 500;
    private readonly int[] _itemIds = { 1000, 1001, 1002 };

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        RefreshCoins();
    }

    public void RefreshCoins()
    {
        if (!_hasAddedScore)
        {
            _currentCoins = DataManager.Instance.playerCurrency.coins + Mathf.RoundToInt(Score.score);
            _hasAddedScore = true;
        }
        else
        {
            _currentCoins = DataManager.Instance.playerCurrency.coins;
        }
        
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

        _currentCoins -= _costPerDraw;
        DataManager.Instance.playerCurrency.coins = _currentCoins;
        DataManager.Instance.SaveAllDynamicData();

        int randomIndex = Random.Range(0, _itemIds.Length);
        int chosenItemId = _itemIds[randomIndex];

        InventoryManager.Instance.AddItem(chosenItemId);
        
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