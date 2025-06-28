using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreDisplay : MonoBehaviour
{
    [Header("����")]
    public int score = 0;

    [Header("����Ч��")]
    public Transform counterUI; 
    public float shakeDuration = 0.5f;
    public float shakeStrength = 10f;
    private int _lastScore;

    private Text _text;

    void Start()
    {
        // ��ʼ���ı����
        _text = GetComponent<Text>();
        _lastScore = score;
        UpdateScoreDisplay();
    }

    // ���ӷ���
    public void AddScore(int points)
    {
        UpdateScoreDisplay();
    }

    // ����UI��ʾ
    private void UpdateScoreDisplay()
    {
        _text.text = $"{score.ToString()}";
    }



}