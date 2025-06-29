using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))] // �Զ�����������
public class TimelineEndCallback : MonoBehaviour
{
    void Start()
    {
        // ��ȡTimeline��Director���
        PlayableDirector director = GetComponent<PlayableDirector>();

        // ע������¼�
        director.stopped += OnTimelineFinished;
    }

    // Timeline����ʱ�Զ�����
    private void OnTimelineFinished(PlayableDirector _)
    {
        // ֱ�ӵ���Ŀ�귽��
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartLog();
        }
        else
        {
            Debug.LogError("GameManagerʵ��δ�ҵ���");
        }
    }
}