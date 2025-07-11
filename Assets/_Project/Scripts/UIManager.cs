using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] public UIMessagePanel UIMessagePanel;
    [SerializeField] public UISystemMessage UISystemMessagePanel;
    [SerializeField] public UITechLevel TechLevelPanel;
    [SerializeField] public UILevelUpPanel UILevelUpPanel;

    protected override void Awake()
    {
        base.Awake();

        //if(itemUITipDatabase != null)
        //    InitMessageUI(itemUITipDatabase);
        DontDestroyOnLoad(gameObject);
    }
    public void FadeUIDuration(CanvasGroup UIPanel, float fadeStrength, float duration)
    {
        UIPanel.gameObject.SetActive(true);
        UIPanel.DOFade(fadeStrength, duration);
    }

    public void RaiseMessageUI()
    {
        UIMessagePanel.gameObject.SetActive(true);
    }

    public void InitMessageUI(itemUITipDatabase itemUITipDatabase)
    {
        UIMessagePanel.InitMessages(itemUITipDatabase);
    }

}
