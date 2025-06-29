using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    private Text timerText;
    private float timer = 0f;

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
        timer += Time.deltaTime;
        int seconds = Mathf.FloorToInt(timer);
        timerText.text = seconds.ToString();
    }
}
