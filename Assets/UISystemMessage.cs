using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemMessage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform scroll;
    [SerializeField] public GameObject messageContainerPfb;

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

        GameObject newMessage = Instantiate(messageContainerPfb, scroll);
        newMessage.GetComponent<UISystemMessageTip>().Show(text, d);
        return;
    }

    public void ForceClosePanel()
    {
        Config.RemoveAllChildren(scroll.gameObject);
        StopAllCoroutines();
    }
}