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
        if (Score.coinCount >= maxCoinCount)
        {
            Debug.Log("�Ѵﵽ�������������: " + maxCoinCount);
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
            //Debug.Log("���ɽ�ң���ǰ����: " + coinCount);
        }
        else
        {
            Debug.LogError("CoinPrefabδ��ֵ!");
        }
    }

    public void CreateDeadCoin(Vector3 position, int bigCoinCount)
    {
        int randomNumber = Random.Range(1, 4);
        if(randomNumber ==1)
            AudioManager.S.PlayFX(ghostDie1, 1.5f, 1f);
        else if(randomNumber ==2)
            AudioManager.S.PlayFX(ghostDie2, 1.5f, 1f);
        else if(randomNumber ==3)
            AudioManager.S.PlayFX(ghostDie3, 1.5f, 1f);
        for (int i = 0; i < bigCoinCount; i++)
        {
            GameObject newCoin = Instantiate(bigCoinPrefab, position, Quaternion.identity);
            CoinController newCoinScript = newCoin.GetComponent<CoinController>();
        }
    }
}
