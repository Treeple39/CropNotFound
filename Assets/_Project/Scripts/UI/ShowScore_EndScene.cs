using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowScore_EndScene : MonoBehaviour
{
    private Text _text;
    [SerializeField] private bool update;
    private NumberCounterTMP numberCounterTMP;
    private NumberCounter numberCounter;

    void Awake()
    {
        _text = GetComponent<Text>();
        if (numberCounterTMP == null || numberCounter == null)
        {
            TryGetComponent<NumberCounterTMP>(out numberCounterTMP);
            TryGetComponent<NumberCounter>(out numberCounter);
        }
    }

    private void OnEnable()
    {
        ShowScore();
    }

    private void FixedUpdate()
    {
        if (update)
        {
            _text.text = Score.score.ToString();
        }
    }

    void ShowScore()
    {
        if(numberCounterTMP != null)
            numberCounterTMP.SetValue((int)Score.score);
        else if(numberCounter != null)
            numberCounter.SetValue((int)Score.score);
        else
            _text.text = Score.score.ToString();
    }
}
