using UnityEngine;
using UnityEngine.UI;

public class UIButtonGotoDrawCards : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnDrawCardsButtonClicked);
        }
        else
        {
            Debug.LogWarning("UIButtonGotoDrawCards: 没有在按钮上找到 Button 组件！");
        }
    }

    private void OnDrawCardsButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToCardScene();
        }
        else
        {
            Debug.LogWarning("UIButtonGotoDrawCards: 找不到 GameManager 单例！");
        }
    }
}