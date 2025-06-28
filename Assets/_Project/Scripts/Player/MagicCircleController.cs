// 文件名: MagicCircleController.cs (快速变白版)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MagicCircleController : MonoBehaviour
{
    [Header("预制体引用")]
    public GameObject magicCirclePrefab;
    public GameObject spawnEffectPrefab;

    [Header("动画效果参数")]
    public float fadeDuration = 0.5f;
    public float sizeMultiplier = 1.0f;
    public float rotationSpeed = 15.0f;

    
    [Header("颜色渐变参数")]
    [Tooltip("魔法阵从原始颜色完全变为白色所需的时间(秒)")]
    public float timeToBecomeWhite = 1.0f;


    public void SpawnMagicCircle(List<Vector3> loopPoints, float lifetime)
    {
        if (magicCirclePrefab == null)
        {
            Debug.LogError("MagicCircleController: 未指定 magicCirclePrefab！");
            return;
        }
        if (loopPoints == null || loopPoints.Count < 3) return;

        Vector3 centerPoint = CalculateCenterPoint(loopPoints);
        float radius = CalculateAverageRadius(loopPoints, centerPoint);

        Canvas worldCanvas = FindObjectOfType<Canvas>();
        if (worldCanvas == null || worldCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("请在场景里放一个 World-Space 模式的 Canvas");
            return;
        }

        GameObject circleInstance = Instantiate(
          magicCirclePrefab,
          centerPoint,
          Quaternion.identity,
          worldCanvas.transform
        );

        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, centerPoint, Quaternion.identity);
        }

        StartCoroutine(MagicCircleLifecycle(circleInstance, lifetime, radius));
    }

    private IEnumerator MagicCircleLifecycle(GameObject circleInstance, float lifetime, float radius)
    {
        Image circleImage = circleInstance.GetComponent<Image>();
        if (circleImage == null)
        {
            Debug.LogError("MagicCircle Prefab上缺少Image组件！");
            yield break;
        }

        Vector3 finalScale = Vector3.one * (radius * 2 * sizeMultiplier);

        // --- 阶段1: 出现 ---
        float timer = 0f;
        Color originalColor = circleImage.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        while (timer < fadeDuration)
        {
            float progress = Mathf.Clamp01(timer / fadeDuration);
            circleImage.color = Color.Lerp(transparentColor, originalColor, progress);
            circleInstance.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, progress);
            timer += Time.deltaTime;
            yield return null;
        }
        circleInstance.transform.localScale = finalScale;
        circleImage.color = originalColor;

        // --- 阶段2: 激活 (旋转并快速变白) ---
        float activeDuration = lifetime - (fadeDuration * 2);
        if (activeDuration > 0)
        {
            float activeTimer = 0f;
            Color targetWhiteColor = Color.white;

            while (activeTimer < activeDuration)
            {
                // 1. 持续旋转
                circleInstance.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

                // ★★★★★【核心修改】★★★★★
                // 2. 颜色渐变
                // 现在的进度是基于我们设定的 timeToBecomeWhite，而不是总的激活时间
                float colorProgress = Mathf.Clamp01(activeTimer / timeToBecomeWhite);
                circleImage.color = Color.Lerp(originalColor, targetWhiteColor, colorProgress);
                // ★★★★★★★★★★★★★★★★★★★

                activeTimer += Time.deltaTime;
                yield return null;
            }
        }

        // --- 阶段3: 消失 ---
        timer = 0f;
        Color finalColorBeforeFade = circleImage.color;
        Color finalTransparentColor = new Color(finalColorBeforeFade.r, finalColorBeforeFade.g, finalColorBeforeFade.b, 0);
        while (timer < fadeDuration)
        {
            float progress = Mathf.Clamp01(timer / fadeDuration);
            circleImage.color = Color.Lerp(finalColorBeforeFade, finalTransparentColor, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        // --- 阶段4: 销毁 ---
        Destroy(circleInstance);
    }

    // --- 计算方法 (保持不变) ---
    private Vector3 CalculateCenterPoint(List<Vector3> points)
    {
        if (points == null || points.Count == 0) return Vector3.zero;
        Vector3 sum = Vector3.zero;
        foreach (var point in points) sum += point;
        return sum / points.Count;
    }

    private float CalculateAverageRadius(List<Vector3> points, Vector3 center)
    {
        if (points == null || points.Count == 0) return 0f;
        float totalDistance = 0f;
        foreach (var point in points)
        {
            totalDistance += Vector3.Distance(point, center);
        }
        return totalDistance / points.Count;
    }
}