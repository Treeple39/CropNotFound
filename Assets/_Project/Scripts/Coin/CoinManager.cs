using Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CoinManager : Singleton<CoinManager>
{
    [SerializeField]
    public AudioClip ghostDie1;
    public AudioClip ghostDie2;
    public AudioClip ghostDie3;

    [SerializeField] public GameObject coinPrefab; // ���Ԥ����
    [SerializeField] public GameObject bigCoinPrefab;
    [SerializeField] public GameObject ItemBasePrefab;
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
        }
        else if (_instance != this)
        {
        }
    }

    public void Update()
    {
        if (Score.coinCount >= maxCoinCount)
        {
            return;
        }
        else {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName != GameManager.Instance.levelSceneName)
            {
                return;
            }
            CreateNewCoin();
        }
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
            Score.coinCount++;
        }
        else
        {
            Debug.LogError("CoinPrefabδ��ֵ!");
        }
    }

    public void CreateDeadCoin(Vector3 position, int bigCoinCount)
    {
        int randomNumber = Random.Range(1, 4);
        if (randomNumber == 1)
            AudioManager.S.PlayFX(ghostDie1, 1.5f, 1f);
        else if (randomNumber == 2)
            AudioManager.S.PlayFX(ghostDie2, 1.5f, 1f);
        else if (randomNumber == 3)
            AudioManager.S.PlayFX(ghostDie3, 1.5f, 1f);
        GameObject newCoin = Instantiate(bigCoinPrefab, position, Quaternion.identity);
        BigCoin bigCoin = newCoin.GetComponent<BigCoin>();
        bigCoin.scoreCount = bigCoinCount;
    }

    public void CreateDeadItem(Vector3 position)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (Random.value > (float)1/i)
                return;

            int itemID = 1002;
            if (Random.value < 0.3f)
                itemID = 1001;
            else if (Random.value > 0.8f)
                itemID = 1002;
            else
                itemID = 1000;

            GameObject newItem = Instantiate(ItemBasePrefab, position, Quaternion.identity);
            Item item = ItemBasePrefab.GetComponent<Item>();
            item.Init(itemID);
        }
    }
}
