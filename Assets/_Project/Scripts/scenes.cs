using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))] // �Զ������������
public class TimelineEndCallback1 : MonoBehaviour
{
    void Start()
    {
        PlayableDirector director = GetComponent<PlayableDirector>();

        director.stopped += OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector _)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("GameManagerʵ��δ�ҵ���");
        }
    }
}