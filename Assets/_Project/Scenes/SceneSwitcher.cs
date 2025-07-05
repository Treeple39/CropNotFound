using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector))]
public class OpeningAnimationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button skipButton; // ��קUI������ť��Inspector

    private PlayableDirector timelineDirector;

    private void Awake()
    {
        // ��ȡTimeline���
        timelineDirector = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        // ��ʼ��������ť״̬
        skipButton.gameObject.SetActive(ShouldShowSkipButton());

        // ��Timeline�����¼�
        timelineDirector.stopped += OnTimelineFinished;

        // ��������ť�¼�
        skipButton.onClick.AddListener(SkipAnimation);
    }

    private void Update()
    {
        // ��������ť�ɼ�ʱ��������ⰴ������
        if (skipButton.gameObject.activeSelf && Input.anyKeyDown)
        {
            SkipAnimation();
        }
    }

    /// <summary>
    /// �ж��Ƿ�Ӧ����ʾ������ť
    /// </summary>
    private bool ShouldShowSkipButton()
    {
        return DataManager.Instance.HasSeenOpeningAnimation();
    }

    /// <summary>
    /// Timeline���Ž���ʱ�Զ�����
    /// </summary>
    private void OnTimelineFinished(PlayableDirector _)
    {
        // �״β���ʱ���Ϊ�ѹۿ�
        if (!DataManager.Instance.HasSeenOpeningAnimation())
        {
            DataManager.Instance.SetHasSeenOpeningAnimation(true);
        }

        // ȷ��GameManager���ں���ת
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("GameManagerʵ��δ�ҵ���");
        }
    }

    /// <summary>
    /// �ֶ���������
    /// </summary>
    private void SkipAnimation()
    {
        // �������Ϊ�ѹۿ�����ʹδ�����꣩
        DataManager.Instance.SetHasSeenOpeningAnimation(true);

        // ��ת�����˵�
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
        else
        {
            Debug.LogError("��תʧ�ܣ�GameManagerδ��ʼ��");
        }
    }

    private void OnDestroy()
    {
        // �����¼���
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }
}