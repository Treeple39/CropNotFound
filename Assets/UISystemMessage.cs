using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemMessage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public Text messageText;
    [SerializeField] public GameObject messageContainer;
    [SerializeField] private Animator anim;

    private void OnEnable()
    {
        EventHandler.OnSystemMessageShow += OnShowSystemMessage;
    }

    private void OnDisable()
    {
        EventHandler.OnSystemMessageShow -= OnShowSystemMessage;
    }

    private void OnShowSystemMessage(string text, float d)
    {
        if (text == null)
        {
            return;
        }

        if (messageContainer.activeSelf)
        {
            ForceClosePanel();
        }

        anim.SetBool("close", false);
        messageContainer.SetActive(true);
        messageText.text = text;

        StartCoroutine(CloseTab(d));
        return;
    }

    private IEnumerator CloseTab(float d)
    {
        yield return new WaitForSecondsRealtime(d);
        anim.SetBool("close", true);
    }

    public void ForceClosePanel()
    {
        anim.SetBool("close", true);
        messageContainer.SetActive(false);
        StopAllCoroutines();
    }
}
