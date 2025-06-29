using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinManager : MonoBehaviour
{
    [SerializeField] public GameObject coinPrefab; // ���Ԥ����
    public int coinCount = 0; // ��ǰ��Ҽ���
    private const int maxCoinCount = 100; // ���������

    [SerializeField] private int spawnAreaMinX = -40; // ��С���ɷ�Χ
    [SerializeField] private int spawnAreaMaxX = 30;  // ������ɷ�Χ
    [SerializeField] private int spawnAreaMinY = -40; // ��С���ɷ�Χ
    [SerializeField] private int spawnAreaMaxY = 10;  // ������ɷ�Χ

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
            Debug.Log("�Ѵﵽ�������������: " + maxCoinCount);
            return;
        }
        else {
            CreateNewCoin();
        }
    }

    public void CreateDeadCoin(Vector3 position)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newCoin = Instantiate(coinPrefab, position, Quaternion.identity);
            CoinController newCoinScript = newCoin.GetComponent<CoinController>();
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
            Debug.Log("���ɽ�ң���ǰ����: " + coinCount);
        }
        else
        {
            Debug.LogError("CoinPrefabδ��ֵ!");
        }
    }

    public void CreateDeadCoin(Vector3 position)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newCoin = Instantiate(coinPrefab, position, Quaternion.identity);
            CoinController newCoinScript = newCoin.GetComponent<CoinController>();
        }
    }
}
