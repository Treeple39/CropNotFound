using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;

    public void Setup(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.color = Color.white;
        bgImage.color = Color.white;
    }
}