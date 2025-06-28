// 文件名: MagicCircleController.cs (已更新)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MagicCircleController : MonoBehaviour
{
    [Header("预制体引用")]
    [Tooltip("魔法阵的UI Image预制体 (必须在World Space Canvas上)")]
    public GameObject magicCirclePrefab;

    [Tooltip("在魔法阵中心释放的粒子特效预制体")]
    public GameObject spawnEffectPrefab;

    [Header("动画效果参数")]
    [Tooltip("魔法阵出现和消失的动画时长")]
    public float fadeDuration = 0.5f;

    [Tooltip("一个乘数，用于调整计算出的半径与最终显示大小的比例")]
    public float sizeMultiplier = 1.0f; // 新增：大小乘数

    /// <summary>
    /// 外部调用的主方法，用于在指定位置生成一个魔法阵。
    /// </summary>
    public void SpawnMagicCircle(List<Vector3> loopPoints, float lifetime)
    {
        if (magicCirclePrefab == null)
        {
            Debug.LogError("MagicCircleController: 未指定 magicCirclePrefab！");
            return;
        }

        if (loopPoints == null || loopPoints.Count < 3) return; // 点太少无法形成图形

        // 1. 计算中心点和半径
        Vector3 centerPoint = CalculateCenterPoint(loopPoints);
        float radius = CalculateAverageRadius(loopPoints, centerPoint); // 【新增】计算半径

        // 找到场景中 World-Space Canvas
        Canvas worldCanvas = FindObjectOfType<Canvas>();
        if (worldCanvas == null || worldCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("请在场景里放一个 World-Space 模式的 Canvas");
            return;
        }
        // parent 到 Canvas 下
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

        // 3. 启动完整的生命周期协程，并将半径传递过去
        StartCoroutine(MagicCircleLifecycle(circleInstance, lifetime, radius));
    }

    /// <summary>
    /// 管理单个魔法阵从出现到消失的整个生命周期。
    /// </summary>
    private IEnumerator MagicCircleLifecycle(GameObject circleInstance, float lifetime, float radius)
    {
        Image circleImage = circleInstance.GetComponent<Image>();
        if (circleImage == null)
        {
            Debug.LogError("MagicCircle Prefab上缺少Image组件！");
            yield break;
        }

        // 【修改】计算最终的目标大小
        // 我们假设预制体的基础大小是1x1，所以直接用半径乘以乘数
        // 注意：UI Image的大小通常由RectTransform的width/height决定，但对于World Space UI，
        // 用localScale来控制大小更方便，因为它和世界单位直接对应。
        Vector3 finalScale = Vector3.one * (radius * 2 * sizeMultiplier); // 直径 = 半径 * 2

        // --- 阶段1: 出现动画 (淡入 + 放大) ---
        float timer = 0f;
        Color startColor = circleImage.color;
        startColor.a = 0; // 确保开始时是透明的

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeDuration);

            // 淡入
            circleImage.color = new Color(startColor.r, startColor.g, startColor.b, progress);
            // 从小变大，目标是我们计算出的finalScale
            circleInstance.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, progress);

            yield return null;
        }

        // 确保最终大小和颜色正确
        circleInstance.transform.localScale = finalScale;
        circleImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f);


        // --- 阶段2: 保持显示 ---
        float remainingTime = lifetime - (fadeDuration * 2);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // --- 阶段3: 消失动画 (淡出) ---
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = 1f - Mathf.Clamp01(timer / fadeDuration);

            // 淡出
            circleImage.color = new Color(startColor.r, startColor.g, startColor.b, progress);

            yield return null;
        }

        // --- 阶段4: 销毁 ---
        Destroy(circleInstance);
        Debug.Log("Destroy(circleInstance);");
    }

    /// <summary>
    /// 计算一组点的几何中心。
    /// </summary>
    private Vector3 CalculateCenterPoint(List<Vector3> points)
    {
        if (points == null || points.Count == 0) return Vector3.zero;
        Vector3 sum = Vector3.zero;
        foreach (var point in points) sum += point;
        return sum / points.Count;
    }

    /// <summary>
    /// 【新增】计算一组点相对于其中心的平均半径。
    /// </summary>
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