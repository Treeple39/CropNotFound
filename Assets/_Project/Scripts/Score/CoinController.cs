using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private Coin coin;
    private bool findPlayer = false;
    public Enemy enemy;
    private void Start()
    {
        enemy = GetComponent<Enemy>();
        this.anim = GetComponent<Animator>();
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
            SetAnimatorBool("Die",true);
        }
    }
    public void SetAnimatorBool(string parameterName, bool value)
    {
        if (anim != null)
        {
            anim.SetBool(parameterName, value);
        }
        else
        {
            Debug.LogWarning("Animator组件未找到！");
        }
    }
}
