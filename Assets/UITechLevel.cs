using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITechLevel : MonoBehaviour
{
    [SerializeField] private Text techLevel;
    [SerializeField] private Image pointsSlider;

    [HideInInspector] public float pointsLimit;

    public void AddPointsUI(float points)
    {
        if(points < pointsLimit)
            pointsSlider.DOFillAmount(points / pointsLimit, 0.5f);
    }

    public void LevelUpUI(int newLevel, float remainPoints)
    {
        pointsSlider.DOFillAmount(1, 0.2f).OnComplete(() => 
        {
            pointsSlider.fillAmount = 0;
            techLevel.text = newLevel.ToString();
            AddPointsUI(remainPoints); 
        });
    }

    public void InitTechLevelUI(int newLevel, float points)
    {
        techLevel.text = newLevel.ToString();
        pointsSlider.fillAmount = points / pointsLimit;
    }

}
