using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreDisplay : MonoBehaviour
{
    [Header("分数")]
    public int score = 0;

    [Header("颤动效果")]
    public Transform counterUI; 
    public float shakeDuration = 0.5f;
    public float shakeStrength = 10f;
    private int _lastScore;

    private Text _text;

    void Start()
    {
        // 初始化文本组件
        _text = GetComponent<Text>();
        _lastScore = score;
        UpdateScoreDisplay();
    }

    // 增加分数
    public void AddScore(int points)
    {
        UpdateScoreDisplay();
    }

    // 更新UI显示
    private void UpdateScoreDisplay()
    {
        _text.text = $"{score.ToString()}";
    }



}