using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 确保这个脚本挂载的对象上有一个Image组件
[RequireComponent(typeof(Image))]
public class FadeInOnStart : MonoBehaviour
{
    [Header("淡入效果设置")]
    [Tooltip("淡入效果持续的时间（秒）")]
    public float fadeDuration = 2.0f;

    [Tooltip("淡入效果开始前的延迟时间（秒）")]
    public float startDelay = 1.0f;

    private Image darkImage;

    void Start()
    {
        darkImage = GetComponent<Image>();

        // 确保开始时图片是完全透明的
        Color startColor = darkImage.color;
        startColor.a = 0.0f; // Alpha值为0代表透明
        darkImage.color = startColor;

        // 启动我们的淡入协程
        StartCoroutine(FadeToOpaqueCoroutine());
    }

    /// <summary>
    /// 一个协程，负责将图片的透明度从0（透明）平滑地过渡到1（不透明）。
    /// </summary>
    private IEnumerator FadeToOpaqueCoroutine()
    {
        // 1. 等待开始前的延迟
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // 2. 执行淡入动画
        float timer = 0f;
        Color currentColor = darkImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            // 从 0.0 (透明) 插值到 1.0 (不透明)
            float newAlpha = Mathf.Lerp(0.0f, 1.0f, timer / fadeDuration);
            
            currentColor.a = newAlpha;
            darkImage.color = currentColor;

            yield return null;
        }

        // 3. 【修正】动画结束后，确保最终的Alpha值为1.0
        currentColor.a = 1.0f;
        darkImage.color = currentColor;

        // 对于“从看不见到看见”的淡入效果，通常我们不希望它在结束后消失
        // 所以这里的 gameObject.SetActive(false) 应该保持注释或删除
    }
}