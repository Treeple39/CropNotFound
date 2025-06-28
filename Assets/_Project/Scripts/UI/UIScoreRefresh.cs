using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class ScoreDisplay : MonoBehaviour
{
    [Header("��������")]
    public int targetScore = 0;  // Ŀ�����
    public float scoreLerpSpeed = 20f; // �����仯�ٶ�

    [Header("�ζ�Ч��")]
    public Transform counterUI;  
    public float shakeIntensity = 3f; // �ζ�����
    public float shakeFrequency = 50f; // �ζ�Ƶ��

    private Text _text;
    private int _displayedScore; // ��ǰ��ʾ�ķ���
    private Coroutine _updateRoutine;

    void Start()
    {
        _text = GetComponent<Text>();
        _displayedScore = targetScore;
        UpdateScoreDisplay();
    }

    // �ⲿ���ô˷����޸ķ���
    public void SetTargetScore(int newScore)
    {
        targetScore = newScore;
        if (_updateRoutine != null) StopCoroutine(_updateRoutine);
        _updateRoutine = StartCoroutine(UpdateScoreWithEffects());
    }

    // ��Ч���ķ�������Э��
    private IEnumerator UpdateScoreWithEffects()
    {
        Vector3 originalPos = counterUI.position;
        float shakeTimer = 0f;

        // �����仯�ڼ�����ζ�
        while (_displayedScore != targetScore)
        {
            // ������ֵԽ�󣬱仯Խ��
            int delta = targetScore - _displayedScore;
            _displayedScore += (int)(Mathf.Sign(delta) * Mathf.Min(scoreLerpSpeed * Time.deltaTime, Mathf.Abs(delta)));

            // ��Ƶ���»ζ�
            shakeTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(shakeTimer * shakeFrequency * Mathf.PI * 2) * shakeIntensity;
            counterUI.position = originalPos + new Vector3(0, yOffset, 0);

            UpdateScoreDisplay();
            yield return null;
        }

        // �ָ�ԭλ
        counterUI.position = originalPos;
    }

    private void UpdateScoreDisplay()
    {
        _text.text = _displayedScore.ToString();
    }
}