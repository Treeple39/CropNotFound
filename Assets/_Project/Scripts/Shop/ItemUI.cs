using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;

    public void Setup(Sprite icon, Color color)
    {
        iconImage.sprite = icon;
        iconImage.color = color;
        bgImage.color = color;
    }
}