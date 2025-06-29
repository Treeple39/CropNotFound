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
    [SerializeField] AudioManager audioManager;


    private bool destoryBool = false;

    public AudioClip coinGet;


    //生成硬币
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

    //寻找玩家
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

        // 计算与玩家的距离
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // 如果距离大于最小停止距离，则继续移动
        if (distance > minDistance)
        {
            // 计算移动方向
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
    /// 删除硬币自身
    /// </summary>
    public void DestroySelf()
    {
        if (!destoryBool)
        {
            //_instance.CoinDestroyed();
            destoryBool = true;
            
        }

       // audioManager.PlayFX(coinGet, .5f, .5f);
        Destroy(this.gameObject);
    }


}
