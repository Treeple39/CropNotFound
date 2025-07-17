using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Coin : MonoBehaviour
{
    [Header("Coin info")]
    [SerializeField] float detectRange = 3;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float minDistance = 0.5f;


    private bool destoryBool = false;

    public AudioClip coinGet;


    //����Ӳ��
    /*public void CreateCoin()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaMinX, spawnAreaMaxX),
            Random.Range(spawnAreaMinY, spawnAreaMaxY),
            0f
        );
        GameObject newCoin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        CoinController newCoinScript = newCoin.GetComponent<CoinController>();
    }*/

    //Ѱ�����
    public bool IsPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance <= detectRange;
        }
        return false;
    }

    public void MoveTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // ��������ҵľ���
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // ������������Сֹͣ���룬������ƶ�
        if (distance > minDistance)
        {
            // �����ƶ�����
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
        else if (distance < minDistance) {
            DestroySelf();
        }
    }

    /// <summary>
    /// ɾ��Ӳ������
    /// </summary>
    public void DestroySelf()
    {
        if (!destoryBool)
        {
            Score.coinCount--;
            Score.score+=10;
            //_instance.CoinDestroyed();
            destoryBool = true;            
        }
        AudioManager.S.PlayFX(coinGet, .5f, 1f);
        Destroy(this.gameObject);
    }


}
