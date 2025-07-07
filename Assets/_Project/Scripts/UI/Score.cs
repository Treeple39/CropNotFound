using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static float score = 0;

    public static int itemCount = 0;

    public static bool skipAnim = false;

    public static int coinCount = 0;
    public static void ResetScore()
    {
        score = 0;
        itemCount = 0;
        coinCount = 0;
    }

    public static void AnimSkip()
    {
        skipAnim = true;
    }
}
