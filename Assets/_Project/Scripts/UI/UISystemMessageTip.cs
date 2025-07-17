using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemMessageTip : MonoBehaviour
{
    [SerializeField] public Text messageText;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
    public void Show(string text, float d)
    {
        if (text == null)
        {
            return;
        }

        anim.SetBool("close", false);
        this.gameObject.SetActive(true);
        messageText.text = text;

        StartCoroutine(CloseTab(d));
        return;
    }

    private IEnumerator CloseTab(float d)
    {
        yield return new WaitForSecondsRealtime(d);
        anim.SetBool("close", true);
    }
}
