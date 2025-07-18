using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyProbabilitCalculator
{
    public static float CalculateDollProbability()
    {
        return 10 - TechLevelManager.Instance.CurrentTechLevel;
    }
    public static float CalculateSlippersProbability()
    {
        if (TechLevelManager.Instance.CurrentTechLevel <= 2) return 0;

        return  TechLevelManager.Instance.CurrentTechLevel * 0.8f;
    }
    public static float CalculateChairProbability()
    {
        if (TechLevelManager.Instance.CurrentTechLevel <= 3) return 0;

        return  TechLevelManager.Instance.CurrentTechLevel * 0.8f;
    }
    public static float CalculateBottleProbability()
    {
        if (TechLevelManager.Instance.CurrentTechLevel <= 4) return 0;

        return  TechLevelManager.Instance.CurrentTechLevel * 0.8f;
    }
    public static float CalculatePillowProbability()
    {
        if (TechLevelManager.Instance.CurrentTechLevel <= 5) return 0;

        return  TechLevelManager.Instance.CurrentTechLevel * 0.8f;
    }    
    public static float CalculateBookProbability()
    {
        if (TechLevelManager.Instance.CurrentTechLevel <= 6) return 0;

        return  TechLevelManager.Instance.CurrentTechLevel * 0.8f;
    }    
}
