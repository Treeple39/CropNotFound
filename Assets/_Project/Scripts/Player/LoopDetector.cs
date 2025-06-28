using System.Collections.Generic;
using UnityEngine;

public class LoopDetector : MonoBehaviour
{
    private class DetectedLoop
    {
        public List<Vector3> Points;
        public float displayTimer;
        private GameObject containerObject; // 保留对容器对象的引用，以便销毁

        public DetectedLoop(GameObject containerObject, List<Vector3> points, float displayTime)
        {
            this.containerObject = containerObject;
            Points = new List<Vector3>(points);
            displayTimer = displayTime;
        }

        public bool Tick(float deltaTime)
        {
            displayTimer -= deltaTime;
            return displayTimer <= 0f;
        }

        public void Destroy()
        {
            if (containerObject != null)
            {
                GameObject.Destroy(containerObject);
            }
        }
    }

    // 敌人状态类保持不变
    private class EnemyState
    {
        public Transform transform;
        public Renderer renderer;
        public Color originalColor;
    }

    [Header("References")]

    public MagicCircleController magicCircleController; // 【新增】对魔法阵控制器的引用
    public List<Transform> enemiesToTrack = new List<Transform>();
    public Color enemyHighlightColor = Color.yellow;

    [Header("Loop Detection")]
    public int minPointsForLoop = 10;
    public float minLoopArea = 1.0f;

    [Header("Visualization")]
    public float loopDisplayTime = 20f;

    private List<DetectedLoop> activeLoops = new List<DetectedLoop>();
    private List<Vector3> tempLoopPoints = new List<Vector3>();
    private List<EnemyState> trackedEnemies = new List<EnemyState>();

    void Start()
    {
        // 【修改】不再需要检查预制体
        // if (loopRendererPrefab == null || loopRendererPrefab.GetComponent<LineRenderer>() == null) ...

        // 缓存敌人状态
        foreach (Transform enemyTransform in enemiesToTrack)
        {
            if (enemyTransform == null) continue;
            Renderer enemyRenderer = enemyTransform.GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                trackedEnemies.Add(new EnemyState
                {
                    transform = enemyTransform,
                    renderer = enemyRenderer,
                    originalColor = enemyRenderer.material.color
                });
            }
        }
    }

    public bool DetectAndCreateLoops(List<Vector3> trailPoints, out int newTrailStartIndex)
    {
        newTrailStartIndex = 0;
        if (trailPoints.Count < minPointsForLoop)
            return false;

        var pts = trailPoints.ToArray();
        int n = pts.Length;
        Vector2 a1 = pts[n - 2], a2 = pts[n - 1];

        for (int i = n - 4; i >= 0; i--)
        {
            Vector2 b1 = pts[i], b2 = pts[i + 1];
            if (SegmentsIntersect(a1, a2, b1, b2))
            {
                tempLoopPoints.Clear();
                for (int j = i + 1; j < n; j++)
                    tempLoopPoints.Add(pts[j]);

                if (CalculateArea(tempLoopPoints) >= minLoopArea)
                {
                    // 【修改】创建一个临时的、不可见的GameObject，而不是从预制体实例化
                    GameObject fakeLoopObject = new GameObject("DetectedLoop_Invisible");
                    fakeLoopObject.transform.SetParent(this.transform);

                    // 创建一个新的DetectedLoop对象并添加到活动列表中
                    activeLoops.Add(new DetectedLoop(fakeLoopObject, tempLoopPoints, loopDisplayTime));

                    HighlightEnemies();

                    if (magicCircleController != null)
                    {
                        Debug.Log("magicCircleController!");
                        magicCircleController.SpawnMagicCircle(tempLoopPoints, loopDisplayTime);
                    }
                    newTrailStartIndex = i + 1;
                    return true;
                }
            }
        }

        return false;
    }

    void Update()
    {
        if (activeLoops.Count == 0) return;
        bool loopsWereRemoved = false;
        for (int i = activeLoops.Count - 1; i >= 0; i--)
        {
            if (activeLoops[i].Tick(Time.deltaTime))
            {
                activeLoops[i].Destroy();
                activeLoops.RemoveAt(i);
                loopsWereRemoved = true;
            }
        }
        if (loopsWereRemoved)
        {
            HighlightEnemies();
        }
    }

    private void HighlightEnemies()
    {
        foreach (var enemyState in trackedEnemies)
        {
            if (enemyState.renderer != null)
                enemyState.renderer.material.color = enemyState.originalColor;
        }

        foreach (var loop in activeLoops)
        {
            foreach (var enemyState in trackedEnemies)
            {
                if (enemyState.renderer.material.color == enemyHighlightColor) continue;
                Vector2 enemyPosition = enemyState.transform.position;
                if (IsPointInPolygon(enemyPosition, loop.Points))
                {
                    enemyState.renderer.material.color = enemyHighlightColor;
                }
            }
        }
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