using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class NumberCounter : MonoBehaviour
{
    public Text targetText;           // �󶨵�UI Text
    public float countDuration = 0.5f; // ���ֱ仯����ʱ��
    public float shakeStrength = 5f;   // �������ȣ����أ�
    public float shakeFrequency = 25f; // ����Ƶ��
    public bool shakeDirectionX; // ����Ƶ��
    public bool shakeDirectionY; // ����Ƶ��
    public bool playShake = true;      // �Ƿ񶶶�

    private int currentValue;
    private Coroutine countCoroutine;
    private Vector3 originalPos;

    void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<Text>();

        originalPos = targetText.rectTransform.localPosition;
    }

    private void Start()
    {
        SetValue(150);
    }

    /// <summary>
    /// Set new aim number;
    /// </summary>
    public void SetValue(int newValue)
    {
        if (!int.TryParse(targetText.text, out currentValue)) 
        {
            currentValue = newValue;
        }

        if (countCoroutine != null)
            StopCoroutine(countCoroutine);

        countCoroutine = StartCoroutine(CountToValue(newValue));
    }

    IEnumerator CountToValue(int targetValue)
    {
        int startValue = currentValue;
        float timer = 0f;
        Vector3 vector = Vector3.zero;

        while (timer < countDuration)
        {
            timer += Time.deltaTime;
            float t = timer / countDuration;
            currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
            targetText.text = currentValue.ToString();

            if (playShake)
            {
                float shakeOffset = Mathf.Sin(Time.time * shakeFrequency) * shakeStrength * (1 - t);
                if(shakeDirectionX)
                {
                    if(shakeDirectionY)
                    {
                        vector = new Vector3(shakeOffset, shakeOffset, 0);
                    }
                    vector = new Vector3(shakeOffset, 0, 0);
                }
                else if(shakeDirectionY) 
                {
                    vector = new Vector3(0, shakeOffset, 0);
                }
                targetText.rectTransform.localPosition = originalPos + vector;
            }

            yield return null;
        }

        currentValue = targetValue;
        targetText.text = currentValue.ToString();
        targetText.rectTransform.localPosition = originalPos;
    }
}
