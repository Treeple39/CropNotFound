using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] public GameObject coinPrefab; // 金币预制体
    public int coinCount = 0; // 当前金币计数
    private const int maxCoinCount = 100; // 最大金币数量

    [SerializeField] private int spawnAreaMinX = -40; // 最小生成范围
    [SerializeField] private int spawnAreaMaxX = 30;  // 最大生成范围
    [SerializeField] private int spawnAreaMinY = -40; // 最小生成范围
    [SerializeField] private int spawnAreaMaxY = 10;  // 最大生成范围

    public static CoinManager _instance;
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (coinCount >= maxCoinCount)
        {
            Debug.Log("已达到最大金币数量限制: " + maxCoinCount);
            return;
        }
        else {
            CreateNewCoin();
        }
    }

    public void CoinDestroyed()
    {
        coinCount--;
    }

    private void CreateNewCoin()
    {
        if (coinPrefab != null)
        {
            Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaMinX, spawnAreaMaxX),
            Random.Range(spawnAreaMinY, spawnAreaMaxY),
            0f
        );
            GameObject newCoin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
            CoinController newCoinScript = newCoin.GetComponent<CoinController>();
            coinCount++;
            Debug.Log("生成金币，当前数量: " + coinCount);
        }
        else
        {
            Debug.LogError("CoinPrefab未赋值!");
        }
    }
}
