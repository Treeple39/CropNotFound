using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private Text rarityText;
    [SerializeField] private Text itemAmount;
    [SerializeField] private Text itemText;
    [SerializeField] private GameObject Pack;

    public void Setup(Sprite icon, Color color, int amount, RarityData rarity, string name = "?", bool open = false)
    {
        iconImage.sprite = icon;
        iconImage.color = color;
        rarityText.color = rarity.outlineColor;
        bgImage.color = color;
        itemAmount.text = amount.ToString();
        itemText.text = name.ToString();

        Pack.SetActive(!open);

        if(open)
        UIManager.Instance.RewardFxImagePosSet.Add(SaveImagePos(icon, rarity));
    }

    public FxImagePos SaveImagePos(Sprite icon, RarityData rarity)
    {
        FxImagePos data = new();

        data.image = icon;
        data.pos = this.transform.position;
        data.Rarity = rarity;

        return data;
    }
}