using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyShopGoods : MonoBehaviour
{
    public Image icon;
    public ParticleSystem fxGlow;

    public void Setup(FxImagePos fxImagePos)
    {
        icon.sprite = fxImagePos.image;
        var main = fxGlow.main;
        main.startColor = fxImagePos.Rarity.outlineColor;
    }
}
