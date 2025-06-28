using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    private float score;
    public float getScore()
    {
        return score;
    }
    public void setScore(float score)
    {
        this.score = score;
    }
}
