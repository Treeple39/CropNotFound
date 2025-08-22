using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItemCount_EndScene : MonoBehaviour
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

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (update)
        {
            _text.text = Score.itemCount.ToString();
        }
    }

    void ShowScore()
    {
        if (numberCounterTMP != null)
            numberCounterTMP.SetValue((int)Score.itemCount);
        else if (numberCounter != null)
            numberCounter.SetValue((int)Score.itemCount);
        else
            _text.text = Score.itemCount.ToString();
    }
}
