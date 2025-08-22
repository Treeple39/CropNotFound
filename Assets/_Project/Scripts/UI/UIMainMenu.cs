using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���������˵������У��������˵�UI��ť�ĵ���¼���
/// ������� GameManager �ĵ�����ִ�о���ĳ�����ת����Ϸ�߼���
/// </summary>
public class UIMainMenu : MonoBehaviour
{
    /// <summary>
    /// ����ʼ����ť����Ӧ����
    /// ����GameManager��ʼ����/�Ի����̡�
    /// </summary>
    /// 

    public string shopLockMessage;

    public void OnClick_StartGame()
    {
        Debug.Log("UI: ����ˡ���ʼ����ť��");
        // �������GameManager��StartLog()�Ǵ����˵��������ķ���
        GameManager.Instance.StartLog();
    }

    public void OnClick_Main()
    {
        Debug.Log("UI: ����ˡ���ʼ����ť��");
        // �������GameManager��StartLog()�Ǵ����˵��������ķ���
        GameManager.Instance.GoToMainMenu();
    }

    public void OnClick_StartPlay()
    {
        Debug.Log("UI: ����ˡ���ʼ����ť��");
        // �������GameManager��StartLog()�Ǵ����˵��������ķ���
        GameManager.Instance.StartLevel();
    }
    /// <summary>
    /// ��ͼ������ť����Ӧ����
    /// ����GameManagerǰ��ͼ��������
    /// </summary>
    public void OnClick_OpenArchive()
    {
        Debug.Log("UI: ����ˡ�ͼ������ť��");
        UIManager.Instance.SetArchivePanelActive(true);
    }

    /// <summary>
    /// ���˳�����ť����Ӧ����
    /// </summary>
    public void OnClick_ExitGame()
    {
        Debug.Log("UI: ����ˡ��˳�����ť��");
        // ��Unity�༭���У����д��벻����Ч������������Debug.Break()��ģ����ͣ
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // �ڱ�������Ϸ�У����ر���Ϸ����
        Application.Quit();
#endif
    }

    /// <summary>
    /// </summary>
    public void OnClick_OpenSettings()
    {
        UIManager.Instance.SettingPanel.SetActive(true);
    }

    public void OnClick_OpenShop()
    {
        if(TechLevelManager.Instance.CurrentTechLevel >= 10)
            UIManager.Instance.SetShopPanelActive(true);
        else
        {
            EventHandler.CallSystemMessageShow(shopLockMessage, 3f);
        }
    }
}
