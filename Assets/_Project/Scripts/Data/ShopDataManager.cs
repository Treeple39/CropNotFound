using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDataManager : MonoBehaviour
{
    float ShopScore;
    float ScoreUsePerTime = 1000f;




    private void OnEnable()
    {
        ShopScore = Score.score;
    }

    public void drawItem()
    {
        if (ShopScore >= ScoreUsePerTime)
        {
            ShopScore -= ScoreUsePerTime;
            
        }
        else
        {
            Debug.Log("Not Enough Score");
        }
    }

}
