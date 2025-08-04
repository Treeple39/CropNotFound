using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBinder : MonoBehaviour
{
    public string dispatchKey;

    void Start()
    {
        Button btn = GetComponent<Button>();

        if (ButtonDispatcher.Instance.TryGet(dispatchKey, out var action))
        {
            btn.onClick.AddListener(() => action.Invoke());
        }
        else
        {
            Debug.LogWarning($"[ButtonDispatcher] No action found for key: {dispatchKey}", this);
        }
    }
}
