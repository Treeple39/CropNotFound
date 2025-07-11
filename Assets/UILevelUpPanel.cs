using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Button fadeButton;

    public void InitLevel(int sanlevel, int adolevel)
    {
        levelText.text = sanlevel.ToString();
        animEvent.content = adolevel.ToString();
    }

    public void InitContents(int initCount, TechLevelUnlockEventType contentType, int contentID)
    {
        if (AwardContainer.childCount != 0) 
        {
            Config.RemoveAllChildren(AwardContainer.gameObject);
        }

        for (int i = 0; i < initCount; i++)
        {
            LevelUpContentData content = UnlockManager.Instance.GetContentData(contentType, contentID);
            if (content == null)
            {
                continue;
            }

            var contentUI = Instantiate(pfb_ContentContainer, AwardContainer);
            contentUI.GetComponent<UILevelUpContent>().ShowContent(content);
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

}
