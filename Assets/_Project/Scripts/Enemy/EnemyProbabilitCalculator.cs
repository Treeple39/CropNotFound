using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyProbabilitCalculator
{
    public static float CalculateDollProbability()
    {
        return TechLevelManager.Instance.CurrentTechLevel;
    }
    public static float CalculateChairProbability()
    {
        return 1;
    }
    public static float CalculateBottleProbability()
    {
        return 1;
    }
    public static float CalculatePillowProbability()
    {
        return 1;
    }    
    public static float CalculateBookProbability()
    {
        return 1;
    }    
    public static float CalculateSlippersProbability()
    {
        return 1;
    }
}
