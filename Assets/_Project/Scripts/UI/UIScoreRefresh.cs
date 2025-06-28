using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class ScoreDisplay : MonoBehaviour
{
    [Header("分数设置")]
    public int targetScore = 0;  // 目标分数
    public float scoreLerpSpeed = 20f; // 分数变化速度

    [Header("晃动效果")]
    public Transform counterUI;  
    public float shakeIntensity = 3f; // 晃动幅度
    public float shakeFrequency = 50f; // 晃动频率

    private Text _text;
    private int _displayedScore; // 当前显示的分数
    private Coroutine _updateRoutine;

    void Start()
    {
        _text = GetComponent<Text>();
        _displayedScore = targetScore;
        UpdateScoreDisplay();
    }

    // 外部调用此方法修改分数
    public void SetTargetScore(int newScore)
    {
        targetScore = newScore;
        if (_updateRoutine != null) StopCoroutine(_updateRoutine);
        _updateRoutine = StartCoroutine(UpdateScoreWithEffects());
    }

    // 带效果的分数更新协程
    private IEnumerator UpdateScoreWithEffects()
    {
        Vector3 originalPos = counterUI.position;
        float shakeTimer = 0f;

        // 分数变化期间持续晃动
        while (_displayedScore != targetScore)
        {
            // 分数差值越大，变化越快
            int delta = targetScore - _displayedScore;
            _displayedScore += (int)(Mathf.Sign(delta) * Mathf.Min(scoreLerpSpeed * Time.deltaTime, Mathf.Abs(delta)));

            // 高频上下晃动
            shakeTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(shakeTimer * shakeFrequency * Mathf.PI * 2) * shakeIntensity;
            counterUI.position = originalPos + new Vector3(0, yOffset, 0);

            UpdateScoreDisplay();
            yield return null;
        }

        // 恢复原位
        counterUI.position = originalPos;
    }

    private void UpdateScoreDisplay()
    {
        _text.text = _displayedScore.ToString();
    }
}