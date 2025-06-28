using System.Collections.Generic;
using UnityEngine;

public class LoopDetector : MonoBehaviour
{
    // 内部类：记录每个闭环
    private class DetectedLoop
    {
        public List<Vector3> Points;
        public float displayTimer;
        private GameObject containerObject;

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
                GameObject.Destroy(containerObject);
        }
    }

    // 内部类：包装 Enemy 组件和渲染组件
    private class EnemyState
    {
        public Enemy    enemyComponent; // 你的 Enemy 脚本
        public Transform transform;     // 用于位置检测
        public Renderer  renderer;      // 用于高亮
        public Color     originalColor;
    }

    [Header("引用设置")]
    public MagicCircleController magicCircleController;
    [Tooltip("拖入所有要检测的 Enemy 脚本组件")]
    public List<Enemy> enemiesToTrack = new List<Enemy>();
    public Color enemyHighlightColor = Color.yellow;

    [Header("闭环检测")]
    public int   minPointsForLoop = 10;
    public float minLoopArea      = 1.0f;

    [Header("显示参数")]
    public float loopDisplayTime  = 20f;

    private List<DetectedLoop> activeLoops    = new List<DetectedLoop>();
    private List<Vector3>      tempLoopPoints = new List<Vector3>();
    private List<EnemyState>   trackedEnemies = new List<EnemyState>();

    void Start()
    {
        // 初始化每个 EnemyState
        foreach (var enemy in enemiesToTrack)
        {
            if (enemy == null) continue;
            var rend = enemy.GetComponent<Renderer>();
            if (rend == null) continue;
            trackedEnemies.Add(new EnemyState {
                enemyComponent = enemy,
                transform      = enemy.transform,
                renderer       = rend,
                originalColor  = rend.material.color
            });
        }
    }

    /// <summary>
    /// 传入玩家轨迹，检测并生成新环
    /// </summary>
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
                    // 占位用的空 GameObject
                    var go = new GameObject("Loop_Invisible");
                    go.transform.SetParent(transform);
                    activeLoops.Add(new DetectedLoop(go, tempLoopPoints, loopDisplayTime));

                    if (magicCircleController != null)
                        magicCircleController.SpawnMagicCircle(tempLoopPoints, loopDisplayTime);

                    // 高亮并“杀死”被圈住的敌人
                    NotifyAndKillEnemiesInside(tempLoopPoints);

                    newTrailStartIndex = i + 1;
                    return true;
                }
            }
        }

        return false;
    }

    void Update()
    {
        // 更新所有环的存续时间，过期就销毁
        for (int i = activeLoops.Count - 1; i >= 0; i--)
        {
            if (activeLoops[i].Tick(Time.deltaTime))
            {
                activeLoops[i].Destroy();
                activeLoops.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 检测哪些敌人在圈内，给它们高亮并把它们标记为 dead = true
    /// </summary>
    private void NotifyAndKillEnemiesInside(List<Vector3> loopPts)
    {
        // 1) 重置所有敌人颜色
        foreach (var st in trackedEnemies)
            st.renderer.material.color = st.originalColor;

        // 2) 中心点（可选，或直接用各自位置）
        Vector3 center = Vector3.zero;
        foreach (var p in loopPts) center += p;
        center /= loopPts.Count;

        // 3) 对每个敌人做点-多边形检测
        foreach (var st in trackedEnemies)
        {
            Vector2 pos2d = st.transform.position;
            if (IsPointInPolygon(pos2d, loopPts))
            {
                // 高亮
                st.renderer.material.color = enemyHighlightColor;
                // 标记死掉
                st.enemyComponent.dead = true;
                //StarExplode(st.transform.position);
                //StartCoroutine(DelayedDestroy(st.transform, 0.3f));
                // 如果你有 Kill() 方法，也可以改成：
                // st.enemyComponent.Kill();
            }
        }
    }

    // ------ 工具方法 ------

    bool IsPointInPolygon(Vector2 p, List<Vector3> poly)
    {
        bool inside = false;
        int cnt = poly.Count;
        for (int i = 0, j = cnt - 1; i < cnt; j = i++)
        {
            Vector2 vi = poly[i], vj = poly[j];
            bool cond = ((vi.y > p.y) != (vj.y > p.y))
                        && (p.x < (vj.x - vi.x) * (p.y - vi.y) / (vj.y - vi.y) + vi.x);
            if (cond) inside = !inside;
        }
        return inside;
    }

    bool SegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        Vector2 r = a2 - a1, s = b2 - b1;
        float  rs = r.x * s.y - r.y * s.x;
        if (Mathf.Approximately(rs, 0f)) return false;
        Vector2 qp = b1 - a1;
        float  t  = (qp.x * s.y - qp.y * s.x) / rs;
        float  u  = (qp.x * r.y - qp.y * r.x) / rs;
        return t > 0f && t < 1f && u > 0f && u < 1f;
    }

    float CalculateArea(List<Vector3> pts)
    {
        int   cnt  = pts.Count;
        float area = 0f;
        for (int i = 0, j = cnt - 1; i < cnt; j = i++)
            area += pts[j].x * pts[i].y - pts[i].x * pts[j].y;
        return Mathf.Abs(area) * 0.5f;
    }
}