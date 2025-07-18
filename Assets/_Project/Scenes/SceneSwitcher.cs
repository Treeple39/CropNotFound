using UnityEngine;
using UnityEngine.Playables;
using System.Collections; // ����Э������������ռ�

[RequireComponent(typeof(PlayableDirector))]
public class OpeningAnimationManager : MonoBehaviour
{
    private PlayableDirector timelineDirector;

    private void Awake()
    {
        // ��ȡTimeline���
        timelineDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        if (DataManager.Instance.HasSeenOpeningAnimation())
        {
            timelineDirector.Stop();
            // ����һ��Э�̣����ӳٺ���ת����
            StartCoroutine(AutomaticSkipAfterDelay());
        }
        else
        {
            timelineDirector.stopped += OnTimelineFinished;
            timelineDirector.Play();
        }
    }

    /// <summary>
    /// Э�̣���ָ�����ӳٺ���ת�����˵���
    /// ��������Ѿ�������������ʱ�����á�
    /// </summary>
    private IEnumerator AutomaticSkipAfterDelay()
    {
        // �ȴ�4��
        yield return new WaitForSeconds(5f);

        // ��ת�����˵�
        GoToNextScene();
    }

    /// <summary>
    /// ��Timeline�������Ž���ʱ����ϵͳ�Զ����á�
    /// ������ҵ�һ�ιۿ�����ʱ��������
    /// </summary>
    private void OnTimelineFinished(PlayableDirector _)
    {
        // ���Ϊ�ѹۿ�
        DataManager.Instance.SetHasSeenOpeningAnimation(true);

        // ��ת�����˵�
        GoToNextScene();
    }

    /// <summary>
    /// ��װ����ת�������߼������㸴�ò����а�ȫ��顣
    /// </summary>
    private void GoToNextScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("������תʧ�ܣ�GameManager ʵ��δ�ҵ���");
        }
    }

    /// <summary>
    /// �ڶ�������ʱ�������¼��󶨣���ֹ�ڴ�й©��
    /// </summary>
    private void OnDestroy()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }
}