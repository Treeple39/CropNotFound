using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CoinBig : MonoBehaviour
{
    [Header("Coin info")]
    [SerializeField] float detectRange = 3;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float minDistance = 0.5f;


    private bool destoryBool = false;

    public AudioClip coinGet;

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
            Score.score+=50;
            //_instance.CoinDestroyed();
            destoryBool = true;
            CoinManager._instance.CoinDestroyed();
        }
        AudioManager.S.PlayFX(coinGet, .3f, .5f);
        Destroy(this.gameObject);
    }


}
