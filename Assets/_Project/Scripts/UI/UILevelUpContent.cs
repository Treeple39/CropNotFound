using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUpContent : MonoBehaviour
{
    [SerializeField] private Text contentText;
    [SerializeField] private Text contentTitle;
    [SerializeField] private Text contentTypeTip;
    [SerializeField] private Image contentImage;
    [SerializeField] private Image tipImage;
    public void ShowContent(LevelUpContentData data)
    {
        contentText.text = data.contentText;
        contentTitle.text = data.contentTitle;
        contentTypeTip.text = data.contentTypeTip;
        contentImage.sprite = data.contentImage;
        tipImage.color = data.contentTypeColor;
    }
}
