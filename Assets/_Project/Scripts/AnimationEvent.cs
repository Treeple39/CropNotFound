using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    public void DeActivateAfterAnimation()
    {
        this.gameObject.SetActive(false);
    }
}
