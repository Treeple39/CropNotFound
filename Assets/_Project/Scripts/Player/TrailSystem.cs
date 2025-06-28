using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LoopDetector loopDetector;   // 拖入 LoopDetector 的组件引用

    [Header("Trail Settings")]
    public int maxTrailPoints = 100;
    public float minTimeBetweenPoints = 0.05f;
    public float minDistanceBetweenPoints = 0.05f;
    public Color trailColor = Color.cyan;
    public float trailWidth = 0.2f;

    private Queue<Vector3> trailPoints = new Queue<Vector3>();
    private LineRenderer lineRenderer;
    private float lastRecordTime;
    private Vector3 lastRecordPos;

void Start()
{
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.positionCount = 0;
    lineRenderer.loop = false;
    lineRenderer.useWorldSpace = true;
    lineRenderer.startWidth = lineRenderer.endWidth = trailWidth;

    // 1) 强制使用 Sprites/Default，它可以读取顶点色渐变
    Shader spriteShader = Shader.Find("Sprites/Default");
    if (spriteShader == null)
    {
        Debug.LogError("Sprites/Default Shader 丢失！");
        return;
    }
    var mat = new Material(spriteShader);
    mat.color = Color.white;  // 基础色设白
    lineRenderer.material = mat;

    // 2) 再设置白色透明→白色不透明的渐变
    var grad = new Gradient();
    grad.colorKeys = new[] {
        new GradientColorKey(Color.white, 0f),
        new GradientColorKey(Color.white, 1f)
    };
    grad.alphaKeys = new[] {
        new GradientAlphaKey(0f, 0f),  // 最旧的点完全透明
        new GradientAlphaKey(1f, 1f)   // 最新的点完全不透明
    };
    lineRenderer.colorGradient = grad;

    lastRecordPos = player.position;
    lastRecordTime = Time.time;
}

    void Update()
    {
        // 1) 记录轨迹点
        var pos = player.position;
        if (Time.time - lastRecordTime >= minTimeBetweenPoints &&
            Vector3.Distance(pos, lastRecordPos) >= minDistanceBetweenPoints)
        {
            trailPoints.Enqueue(pos);
            if (trailPoints.Count > maxTrailPoints)
                trailPoints.Dequeue();

            lastRecordPos = pos;
            lastRecordTime = Time.time;
            RefreshTrail();
        }

        // 2) 把完整队列传给 LoopDetector 来检查
        if (loopDetector != null)
            loopDetector.DetectLoop(trailPoints);
    }

    private void RefreshTrail()
    {
        var pts = trailPoints.ToArray();
        lineRenderer.positionCount = pts.Length;
        lineRenderer.SetPositions(pts);
    }

    public void ClearTrail()
    {
        trailPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}