using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEvent : MonoBehaviour
{
    [Header("Show Text, If not, Empty")]
    public Text text;
    public string content;

    [Header("Show UFX, If not, Empty")]
    public GameObject UFX;
    public Transform point;
    public Transform mother;
    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    public void DeActivateAfterAnimation()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowTextInAnimation()
    {
        if(text != null)
        {
            text.text = content;
        }
    }

    public void ShowFXInAnimation()
    {
        if (UFX != null && point != null) 
        {
            Instantiate(UFX, point.position, Quaternion.identity, mother);
        }
    }
}
