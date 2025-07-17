using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BigCoinController : MonoBehaviour
{
    private Animation anim;
    private Rigidbody2D rb;
    private Transform player;
    private BigCoin coin;
    private bool findPlayer = false;
    public Enemy enemy;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        coin = GetComponent<BigCoin>();
        if (coin == null)
        {
            Debug.LogError("BigCoin 脚本未挂载到当前GameObject: " + gameObject.name);
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
