using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 挂载在主菜单场景中，处理主菜单UI按钮的点击事件。
/// 它会调用 GameManager 的单例来执行具体的场景跳转或游戏逻辑。
/// </summary>
public class UIMainMenu : MonoBehaviour
{
    /// <summary>
    /// “开始”按钮的响应函数
    /// 调用GameManager开始剧情/对话流程。
    /// </summary>
    public void OnClick_StartGame()
    {
        Debug.Log("UI: 点击了“开始”按钮。");
        // 根据你的GameManager，StartLog()是从主菜单进入剧情的方法
        GameManager.Instance.StartLog();
    }

    /// <summary>
    /// “图鉴”按钮的响应函数
    /// 调用GameManager前往图鉴场景。
    /// </summary>
    public void OnClick_OpenArchive()
    {
        Debug.Log("UI: 点击了“图鉴”按钮。");
        GameManager.Instance.GoToArchive();
    }

    /// <summary>
    /// “退出”按钮的响应函数
    /// </summary>
    public void OnClick_ExitGame()
    {
        Debug.Log("UI: 点击了“退出”按钮。");
        // 在Unity编辑器中，这行代码不会生效，所以我们用Debug.Break()来模拟暂停
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在编译后的游戏中，这会关闭游戏程序
        Application.Quit();
#endif
    }

    /// <summary>
    /// “设置”按钮的响应函数
    /// 注意：你的GameManager中没有处理设置的逻辑。
    /// 通常设置是一个UI面板，而不是一个新场景。
    /// 你需要调用你的UIManager来打开设置面板。
    /// </summary>
    public void OnClick_OpenSettings()
    {
        Debug.Log("UI: 点击了“设置”按钮。");
        // 示例：假设你有一个UIManager可以打开设置面板
        // UIManager.Instance.OpenSettingsPanel();

        // 由于你的GameManager中没有相关代码，这里暂时留空，请根据你的UI框架来补充。
        Debug.LogWarning("设置功能尚未实现，请在 UIManager 中添加打开设置面板的逻辑。");
    }
}
