using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Config : MonoBehaviour
{
    //Image UFX Fundamental Utilities
    public static class ImageUFX
    {
        // UFX_Fade
        public static void UFX_Fade(Image target, float duration, System.Action callback = null)
        {

            target.DOFade(0, duration).OnComplete(() =>
            {
                callback?.Invoke(); 
            });

            return;
        }

        // UFX_Stretch
        public static void UFX_Stretch(Image target, Vector2 endValue, float duration, System.Action callback = null)
        {
            target.color = new Color(1, 1, 1, 0);
            target.DOFade(1, 0.2f);
            target.rectTransform.DOSizeDelta(endValue, duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                callback?.Invoke();
            });

            return;
        }

        public static void UFX_StretchByHeight(Image target, float duration, float lockedHeight = 0, System.Action callback = null)
        {
            target.color = new Color(1, 1, 1, 0);
            target.DOFade(1, 0.2f);

            // Make sure pic's normal hv
            Vector2 size = target.rectTransform.sizeDelta;
            float lockedWidth;
            if (lockedHeight == 0)
            {
                lockedHeight = size.y;
                lockedWidth = size.x;
            }
            else
            {
                lockedWidth = size.x * lockedHeight / size.y;
            }

            target.rectTransform.DOSizeDelta(new Vector2(lockedWidth, lockedHeight), duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                callback?.Invoke();
            });

            return;
        }

        // UFX_Color
        public static void UFX_Color(Image target, Color endColor, float duration, System.Action callback = null)
        {
            if (duration == 0) 
            {
                target.color = endColor;
                return;
            }
            target.DOColor(endColor, duration).OnComplete(() =>
            {
                callback?.Invoke();
            }); 

            return;
        }
    }
}
