using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailSystem : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LoopDetector loopDetector;   // 拖入 LoopDetector 的组件引用

    [Header("Trail Settings")]
    public int   maxTrailPoints       = 100;
    public float pointLifeTime        = 2f;
    public float minTimeBetweenPoints = 0.05f;
    public float minDistanceBetweenPoints = 0.05f;
    public Color trailColor           = Color.cyan;
    public float trailWidth           = 0.2f;

    private struct TrailPoint
    {
        public Vector3 position;
        public float   time;
        public TrailPoint(Vector3 pos, float t) { position = pos; time = t; }
    }
    
    // 【修改】将Queue改为List，因为它更方便地移除范围内的元素
    private List<TrailPoint> trailPoints = new List<TrailPoint>();
    private LineRenderer      lineRenderer;
    private float             lastRecordTime;
    private Vector3           lastRecordPos;
    
    // 【新增】用于传递给LoopDetector的位置列表，避免每帧都创建新列表
    private List<Vector3> positionListForDetector = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.loop          = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth    = lineRenderer.endWidth = trailWidth;

  
        Shader spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader == null)
        {
            Debug.LogError("Sprites/Default Shader 丢失！");
            return;
        }
        var mat = new Material(spriteShader) { color = Color.white };
        lineRenderer.material = mat;

        var grad = new Gradient();
        grad.colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) };
        grad.alphaKeys = new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 1f) };
        lineRenderer.colorGradient = grad;

        lastRecordPos  = player.position;
        lastRecordTime = Time.time;
    }

    void Update()
    {
        float now = Time.time;
        Vector3 pos = player.position;

        // 1) 记录新点
        if (now - lastRecordTime >= minTimeBetweenPoints &&
            Vector3.Distance(pos, lastRecordPos) >= minDistanceBetweenPoints)
        {
            // 【修改】使用List的Add方法
            trailPoints.Add(new TrailPoint(pos, now));
            lastRecordPos  = pos;
            lastRecordTime = now;
        }

        // 2) 剔除旧点
        float expireTime = now - pointLifeTime;
        // 【修改】使用List的RemoveAll，更高效地移除满足条件的旧点
        trailPoints.RemoveAll(p => p.time < expireTime);
        while (trailPoints.Count > maxTrailPoints)
        {
            // 【修改】如果超出最大数量，从列表的开头移除最旧的点
            trailPoints.RemoveAt(0);
        }

        // 3) 刷新可视化
        RefreshTrail();

        if (loopDetector != null && trailPoints.Count > 1)
        {
            // 准备位置列表
            positionListForDetector.Clear();
            foreach (var tp in trailPoints)
            {
                positionListForDetector.Add(tp.position);
            }

            // 调用新的检测方法
            if (loopDetector.DetectAndCreateLoops(positionListForDetector, out int newStartIndex))
            {
                // 保留从交点开始的新轨迹
                if (newStartIndex > 0 && newStartIndex < trailPoints.Count)
                {
                    trailPoints.RemoveRange(0, newStartIndex);
                }
            }
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
        for (int i = 0; i < count; i++)
        {
            pts[i] = trailPoints[i].position;
        }
        
        lineRenderer.positionCount = count;
        //lineRenderer.SetPositions(pts);
    }

    public void ClearTrail()
    {
        trailPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}