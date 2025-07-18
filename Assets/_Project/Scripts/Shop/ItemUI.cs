using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Setup(Sprite icon, string itemName)
    {
        iconImage.sprite = icon;
        nameText.text = itemName;
    }
}