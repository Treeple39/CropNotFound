using UnityEngine;
using TMPro;
using System.Collections;

public class NumberCounterTMP : MonoBehaviour
{
    public TMP_Text tmpText;
    public float countDuration = 0.5f;      // ���ֱ仯����ʱ��

    private int currentValue;
    private float[] charTimeOffsets;        // ÿ���ַ�������λ
    private Coroutine countCoroutine;

    void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();

        tmpText.text = currentValue.ToString();
    }

    public void SetValue(int newValue)
    {
        if (countCoroutine != null)
            StopCoroutine(countCoroutine);

        countCoroutine = StartCoroutine(CountToValue(newValue));
    }

    IEnumerator CountToValue(int targetValue)
    {
        int startValue = currentValue;
        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.deltaTime;
            float t = timer / countDuration;
            currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
            tmpText.text = currentValue.ToString();
            yield return null;
        }

        currentValue = targetValue;
        tmpText.text = currentValue.ToString();
    }
}
