using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BigCoin : MonoBehaviour
{
    [SerializeField] float detectRange = 3;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private GameObject scoreAddPrefab;
    [SerializeField] public int scoreCount = 1;


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

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > minDistance)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
        else if (distance < minDistance)
        {
            ScoreAdd ScoreAdd;
            MeshRenderer renderer = player.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                Vector3 headPos = player.transform.position + Vector3.up * (renderer.bounds.extents.y + 0.3f);

                ScoreAdd = Instantiate(scoreAddPrefab, headPos, Quaternion.identity).GetComponent<ScoreAdd>();
            }
            else
            {
                ScoreAdd = Instantiate(scoreAddPrefab, player.transform.position, Quaternion.identity).GetComponent<ScoreAdd>();
            }
            int totalScore = 50 * scoreCount;
            if (totalScore >= 500)
            {
                ScoreAdd.ShowScore(50 * scoreCount, 1);
            }
            else
            {
                ScoreAdd.ShowScore(50 * scoreCount, 0);
            }


            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!destoryBool)
        {
            Score.score += 50 * scoreCount;

            destoryBool = true;
        }
        AudioManager.S.PlayFX(coinGet, .5f, .5f);
        Destroy(this.gameObject);
    }


}
