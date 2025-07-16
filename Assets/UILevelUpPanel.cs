using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class UILevelUpPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public Text levelText;
    [SerializeField] public Transform AwardContainer;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationEvent animEvent;
    [SerializeField] private GameObject pfb_ContentContainer;
    [SerializeField] private GameObject LevelUpPanel;
    [SerializeField] private Button fadeButton;

    public void InitLevel(int sanlevel, int adolevel)
    {
        levelText.text = sanlevel.ToString();
        animEvent.content = adolevel.ToString();
    }

    public void InitContent(int initCount, List<TechLevelUnlockEventType> contentType, List<int> contentID)
    {
        if (AwardContainer.childCount != 0) 
        {
            Config.RemoveAllChildren(AwardContainer.gameObject);
        }

        for (int i = 0; i < initCount; i++)
        {
            LevelUpContentData content = UnlockManager.Instance.GetContentData(contentType[i], contentID[i]);
            if (content == null)
            {
                continue;
            }

            var contentUI = Instantiate(pfb_ContentContainer, AwardContainer);
            contentUI.GetComponent<UILevelUpContent>().ShowContent(content);
            contentUI.SetActive(false);
        }
    }

    private void ActivateContentContainer()
    {
        if(AwardContainer.childCount != 0)
        {
            for (int i = 0; i < AwardContainer.childCount; i++) 
            {
                AwardContainer.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void RegisterConfirmAction()
    {
        fadeButton.onClick.AddListener(OnConfirmUnlockDetails);
    }

    private void OnConfirmUnlockDetails()
    {
        //触发所有缓存的升级事件
        TechLevelManager.Instance.TriggerPendingUnlockEvents();
        anim.SetBool("close", true);

        fadeButton.onClick.RemoveListener(OnConfirmUnlockDetails);
    }

    public void CloseTab()
    {
        this.gameObject.SetActive(false);
        LevelUpPanel.SetActive(false);
    }

    public void OpenTab()
    {
        this.gameObject.SetActive(true);
        LevelUpPanel.SetActive(true);
        ActivateContentContainer();
    }
}
