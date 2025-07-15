using UnityEngine;
using UnityEngine.UI;

public class UIArchiveButtonHandler : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnArchiveButtonClicked);
        }
        else
        {
            Debug.LogWarning("UIArchiveButtonHandler: 没有在按钮上找到 Button 组件！");
        }
    }

    private void OnArchiveButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            UIManager.Instance.SetArchivePanelActive(true);
        }
        else
        {
            Debug.LogWarning("UIArchiveButtonHandler: 找不到 GameManager 单例！");
        }
    }
}