using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LoopDetector : MonoBehaviour
{
    [Header("References")]
    public Transform enemy;            // 要高亮的目标
    public Color enemyHighlightColor = Color.yellow;

    [Header("Loop Detection")]
    public int minPointsForLoop = 10;
    public float minLoopArea = 1.0f;

    [Header("Visualization")]
    public Color loopColor = Color.red;
    public float loopWidth = 0.1f;
    public float loopDisplayTime = 5.0f;

    private LineRenderer loopRenderer;
    private bool loopDetected;
    private float loopTimer;
    private List<Vector3> loopPoints = new List<Vector3>();
    private Color enemyOriginalColor;
    private Renderer enemyRenderer;

    void Start()
    {
        // 准备 LineRenderer
        loopRenderer = GetComponent<LineRenderer>();
        loopRenderer.positionCount = 0;
        loopRenderer.loop = true;
        loopRenderer.useWorldSpace = true;
        loopRenderer.startWidth = loopRenderer.endWidth = loopWidth;
        loopRenderer.startColor = loopRenderer.endColor = loopColor;
        loopRenderer.enabled = false;

        // 缓存 enemy 原始颜色
        if (enemy != null)
        {
            enemyRenderer = enemy.GetComponent<Renderer>();
            if (enemyRenderer != null)
                enemyOriginalColor = enemyRenderer.material.color;
        }
    }

    /// <summary>
    /// 外部调用：传入当前轨迹点队列
    /// </summary>
    public void DetectLoop(Queue<Vector3> trailPoints)
    {
        if (loopDetected)
            return;

        if (trailPoints.Count < minPointsForLoop)
            return;

        var pts = trailPoints.ToArray();
        int n = pts.Length;
        Vector2 a1 = pts[n - 2], a2 = pts[n - 1];

        // 检查自交
        for (int i = 0; i < n - 3; i++)
        {
            Vector2 b1 = pts[i], b2 = pts[i + 1];
            if (SegmentsIntersect(a1, a2, b1, b2))
            {
                // 提取闭环多边形 (i+1 到 n-1)
                loopPoints.Clear();
                for (int j = i + 1; j < n - 1; j++)
                    loopPoints.Add(pts[j]);

                // 计算面积
                if (CalculateArea(loopPoints) >= minLoopArea)
                {
                    ShowLoop();
                    HighlightEnemy();
                    return;
                }
                break;
            }
        }
    }

    void Update()
    {
        if (!loopDetected)
            return;

        loopTimer -= Time.deltaTime;
        if (loopTimer <= 0f)
            ClearLoop();
    }

    private void ShowLoop()
    {
        loopDetected = true;
        loopTimer = loopDisplayTime;
        loopRenderer.positionCount = loopPoints.Count;
        loopRenderer.SetPositions(loopPoints.ToArray());
        loopRenderer.enabled = true;
    }

    private void ClearLoop()
    {
        loopDetected = false;
        loopRenderer.enabled = false;
        loopPoints.Clear();
        // 恢复 enemy 颜色
        if (enemyRenderer != null)
            enemyRenderer.material.color = enemyOriginalColor;
    }

    private void HighlightEnemy()
    {
        if (enemyRenderer == null)
            return;

        Vector2 e = enemy.position;
        bool inside = IsPointInPolygon(e, loopPoints);
        enemyRenderer.material.color = inside ? enemyHighlightColor : enemyOriginalColor;
    }

    // 射线法点-多边形检测
    bool IsPointInPolygon(Vector2 p, List<Vector3> poly)
    {
        bool inside = false;
        int cnt = poly.Count;
        for (int i = 0, j = cnt - 1; i < cnt; j = i++)
        {
            Vector2 vi = poly[i], vj = poly[j];
            bool test = ((vi.y > p.y) != (vj.y > p.y))
                        && (p.x < (vj.x - vi.x) * (p.y - vi.y) / (vj.y - vi.y) + vi.x);
            if (test) inside = !inside;
        }
        return inside;
    }

    // 2D 线段相交检测
    bool SegmentsIntersect(Vector2 p, Vector2 r, Vector2 q, Vector2 s)
    {
        Vector2 rp = r - p, sq = s - q;
        float rxs = Cross(rp, sq);
        if (Mathf.Approximately(rxs, 0f)) return false;
        Vector2 pq = q - p;
        float t = Cross(pq, sq) / rxs;
        float u = Cross(pq, rp) / rxs;
        return t > 0f && t < 1f && u > 0f && u < 1f;
    }

    float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

    float CalculateArea(List<Vector3> pts)
    {
        int cnt = pts.Count;
        if (cnt < 3) return 0f;
        float area = 0f;
        for (int i = 0, j = cnt - 1; i < cnt; j = i++)
            area += pts[j].x * pts[i].y - pts[i].x * pts[j].y;
        return Mathf.Abs(area) * 0.5f;
    }
}