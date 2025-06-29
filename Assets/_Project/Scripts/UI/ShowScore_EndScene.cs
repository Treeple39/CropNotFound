using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowScore_EndScene : MonoBehaviour
{
    private Text _text;
    
    void Start()
    {
        _text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        ShowScore();
    }

    void ShowScore()
    {
        _text.text = Score.score.ToString();
    }
}
