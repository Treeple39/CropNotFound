using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Animation anim;
    private Rigidbody2D rb;
    private Transform player;
    private Coin coin;
    private bool findPlayer = false;

    private void Start()
    {
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        coin = GetComponent<Coin>();
        if (coin == null)
        {
            Debug.LogError("Coin 脚本未挂载到当前GameObject: " + gameObject.name);
            enabled = false; 
        }
    }

    private void Update()
    {
        if (coin.IsPlayerNearby())
        {
            findPlayer = true;
        }
        if(coin != null && findPlayer)
        {
            coin.MoveTowardsPlayer();
        }
    }
}
