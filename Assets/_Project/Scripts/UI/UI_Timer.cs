using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    private Text timerText;
    private float timer = 0f;
    private float lasttimer = 0f;

    public AudioClip tiktak;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        if (timerText == null)
        {
            Debug.LogError("Text component not found on this GameObject!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer - lasttimer >= 1f)
        {
            lasttimer = timer;
        }
        timer += Time.deltaTime;
        int seconds = Mathf.FloorToInt(timer);
        timerText.text = (60-seconds).ToString();
        if(seconds >= 55)
        {
            timerText.color = Color.red;
            if(timer - lasttimer >= 1f)
            {
                AudioManager.S.PlayFX(tiktak, .5f, .5f);
                lasttimer = timer;
            }
        }
        if(seconds >= 60)
        {
            GameManager.Instance.GoToEndScene();
        }
    }
}
