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

    [Header("set active use")]
    [SerializeField] public GameObject BagPanel;
    [SerializeField] public GameObject ScorePanel;

    public GameObject ArchivePanel;
    public GameObject SettingPanel;


    protected override void Awake()
    {
        base.Awake();
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

    public void ShowHighlight(GameObject panel)
    {
        //Init Shining Obj to Guide the Panel
    }

    public void SetAllUIPanelsActive(bool isActive)
    {
        if (UILevelUpPanel != null)
        {
            UILevelUpPanel.gameObject.SetActive(isActive);
        }

        if (BagPanel != null)
        {
            BagPanel.SetActive(isActive);
        }

        if (ScorePanel != null)
        {
            ScorePanel.SetActive(isActive);
        }
        if (TechLevelPanel != null)
        {
            TechLevelPanel.gameObject.SetActive(isActive);
        }
    }
    public void SetArchivePanelActive(bool isActive)
    {
        if (ArchivePanel != null)
        {
            ArchivePanel.SetActive(isActive);
            EventHandler.CallArchivePanelStateChanged(isActive);
        }
        else
        {
            Debug.LogError("ArchivePanel不存在");
        }
    }
}
