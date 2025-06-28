using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LoopDetector loopDetector;   // 拖入 LoopDetector 的组件引用

    [Header("Trail Settings")]
    public int   maxTrailPoints       = 100;    // 最大点数
    public float pointLifeTime        = 2f;     // 点的最大存活时间（秒）
    public float minTimeBetweenPoints = 0.05f;  // 记录时间间隔
    public float minDistanceBetweenPoints = 0.05f; // 记录距离阈值
    public Color trailColor           = Color.cyan;
    public float trailWidth           = 0.2f;

    // 内部存储：带时间戳的点
    private struct TrailPoint
    {
        public Vector3 position;
        public float   time;
        public TrailPoint(Vector3 pos, float t)
        {
            position = pos;
            time     = t;
        }
    }
    private Queue<TrailPoint> trailPoints = new Queue<TrailPoint>();
    private LineRenderer      lineRenderer;
    private float             lastRecordTime;
    private Vector3           lastRecordPos;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.loop          = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth    = lineRenderer.endWidth = trailWidth;

        // 使用 Sprites/Default Shader 支持顶点色渐变
        Shader spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader == null)
        {
            Debug.LogError("Sprites/Default Shader 丢失！");
            return;
        }
        var mat = new Material(spriteShader) { color = Color.white };
        lineRenderer.material = mat;

        // 白色透明→白色不透明渐变
        var grad = new Gradient();
        grad.colorKeys = new[]
        {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(Color.white, 1f)
        };
        grad.alphaKeys = new[]
        {
            new GradientAlphaKey(0f, 0f), // 最旧的点完全透明
            new GradientAlphaKey(1f, 1f)  // 最新的点完全不透明
        };
        lineRenderer.colorGradient = grad;

        lastRecordPos  = player.position;
        lastRecordTime = Time.time;
    }

    void Update()
    {
        float now = Time.time;
        Vector3 pos = player.position;

        // 1) 根据时间和距离记录新点
        if (now - lastRecordTime >= minTimeBetweenPoints &&
            Vector3.Distance(pos, lastRecordPos) >= minDistanceBetweenPoints)
        {
            trailPoints.Enqueue(new TrailPoint(pos, now));
            lastRecordPos  = pos;
            lastRecordTime = now;
        }

        // 2) 剔除超时或超长的旧点
        float expireTime = now - pointLifeTime;
        while (trailPoints.Count > 0 && trailPoints.Peek().time < expireTime)
            trailPoints.Dequeue();
        while (trailPoints.Count > maxTrailPoints)
            trailPoints.Dequeue();

        // 3) 刷新可视化
        RefreshTrail();

        // 4) 传给 LoopDetector
        if (loopDetector != null)
        {
            // 只取位置部分
            var positions = new Queue<Vector3>();
            foreach (var tp in trailPoints) positions.Enqueue(tp.position);
            loopDetector.DetectLoop(positions);
        }
    }

    private void RefreshTrail()
    {
        int count = trailPoints.Count;
        if (count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        var pts = new Vector3[count];
        int i = 0;
        foreach (var tp in trailPoints)
            pts[i++] = tp.position;

        lineRenderer.positionCount = count;
        lineRenderer.SetPositions(pts);
    }

    /// <summary>
    /// 手动清空轨迹
    /// </summary>
    public void ClearTrail()
    {
        trailPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}