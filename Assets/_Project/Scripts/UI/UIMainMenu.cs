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
    public void OnClick_StartGame()
    {
        Debug.Log("UI: ����ˡ���ʼ����ť��");
        // �������GameManager��StartLog()�Ǵ����˵��������ķ���
        GameManager.Instance.StartLog();
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
    /// �����á���ť����Ӧ����
    /// ע�⣺���GameManager��û�д������õ��߼���
    /// ͨ��������һ��UI��壬������һ���³�����
    /// ����Ҫ�������UIManager����������塣
    /// </summary>
    public void OnClick_OpenSettings()
    {
        Debug.Log("UI: ����ˡ����á���ť��");
        // ʾ������������һ��UIManager���Դ��������
        // UIManager.Instance.OpenSettingsPanel();

        // �������GameManager��û����ش��룬������ʱ���գ���������UI��������䡣
        Debug.LogWarning("���ù�����δʵ�֣����� UIManager �����Ӵ����������߼���");
    }
}
