using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class TextColor
{
    public string text;
    public Color color;
}

public static class Config
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

    public static class Random
    {
        
    }

    public static class ConditionalMath
    {
        public static bool ConditionalMathf(float path1, int sign, float path2)
        {
            switch (sign)
            {
                case 0: //equal
                    if(path1 == path2)
                        return true;
                    break;
                case 1: //greater/equal
                    if (path1 >= path2)
                        return true;
                    break;
                case 2: //lesser/equal
                    if (path1 <= path2)
                        return true;
                    break;
                case 3: //greater
                    if (path1 > path2)
                        return true;
                    break;
                case 4: //lesser
                    if (path1 < path2)
                        return true;
                    break;

                default:
                    break;
            }

            return false;
        }
    }

    public static void RemoveAllChildren(GameObject parent)
    {
        Transform transform;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            transform = parent.transform.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }
}
