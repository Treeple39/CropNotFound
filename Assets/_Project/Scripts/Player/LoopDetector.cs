using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LoopDetector : MonoBehaviour
{
    public GameObject FlashX;
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

    private class TrackableObject
    {
        public GameObject gameObject; 
        public Transform transform;
        public Renderer renderer;
        
        // 如果需要，也可以保留对特定组件的引用
        // public Enemy enemyComponent; 
    }


    [Header("引用设置")]
    // public MagicCircleController magicCircleController; // 【修改】不再需要魔法阵
    [Tooltip("拖入一个挂载了PolygonDrawer脚本的空GameObject预制体")]
    public GameObject polygonDrawerPrefab; // ★★★ 新增 ★★★

    [Tooltip("拖入所有要检测的 Enemy 脚本组件")]
    public List<Enemy> enemiesToTrack = new List<Enemy>();

    [Header("闭环检测")]
    public int minPointsForLoop = 10;
    public float minLoopArea = 1.0f;

    [Header("显示参数")]
    public float loopDisplayTime = 20f;

    private List<DetectedLoop> activeLoops = new List<DetectedLoop>();
    private List<Vector3> tempLoopPoints = new List<Vector3>();
    private List<TrackableObject> trackedObjects = new List<TrackableObject>();


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
                    // ★★★★★【核心修改】★★★★★
                    // 1. 检查预制体是否存在
                    if (polygonDrawerPrefab == null)
                    {
                        Debug.LogError("请在LoopDetector的Inspector中指定PolygonDrawer预制体！");
                        return false;
                    }

                    // 2. 实例化预制体
                    GameObject polygonObject = Instantiate(polygonDrawerPrefab, Vector3.zero, Quaternion.identity);

                    // 3. 获取其上的PolygonDrawer组件
                    PolygonDrawer drawer = polygonObject.GetComponent<PolygonDrawer>();

                    // 4. 调用绘制方法
                    if (drawer != null)
                    {
                        drawer.DrawAndAnimatePulse(tempLoopPoints);
                    }
                    // ★★★★★★★★★★★★★★★★★★★

                    // 高亮并"杀死"被圈住的敌人
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
    /// 检测哪些在ItemGenerator列表中的物体在圈内，并处理它们
    /// </summary>
    private void NotifyAndKillEnemiesInside(List<Vector3> loopPts)
    {
        // ★★★★★【核心修改：直接从 ItemGenerator 获取列表】★★★★★

        // 1. 检查 ItemGenerator 实例是否存在
        if (EnemyGenerator.Instance == null)
        {
            Debug.LogWarning("找不到 ItemGenerator 实例，无法检测物体。");
            return;
        }

        // 2. 直接获取已生成物体的Transform列表
        List<Transform> allSpawnedTransforms = EnemyGenerator.Instance.spawnedTransforms;
        Debug.Log(allSpawnedTransforms);

        // 3. 将Transform列表转换为我们内部的 TrackableObject 结构
        //    这里我们不再需要Select和Where，因为我们可以直接在循环中处理
        List<TrackableObject> objectsToTest = new List<TrackableObject>();
        foreach (Transform itemTransform in allSpawnedTransforms)
        {
            // 安全检查：如果物体在上一轮被销毁了，列表中可能会留下null引用
            if (itemTransform == null) continue;

            var rend = itemTransform.GetComponent<Renderer>();
            // 如果物体有Renderer组件，就把它加入到待检测列表中
            if (rend != null)
            {
                objectsToTest.Add(new TrackableObject
                {
                    gameObject = itemTransform.gameObject,
                    transform = itemTransform,
                    renderer = rend,
                });
            }
        }

        Debug.Log($"从ItemGenerator获取了 {objectsToTest.Count} 个可被圈选的物体进行检测。");

        // 4. 对每个待检测的物体做点-多边形检测
        List<GameObject> objectsToDestroy = new List<GameObject>();
        foreach (var obj in objectsToTest)
        {
            Vector2 pos2d = obj.transform.position;
            if (IsPointInPolygon(pos2d, loopPts))
            {
                Score.itemCount++;
                CoinManager._instance.CreateDeadCoin(obj.transform.position, obj.gameObject.GetComponent<EnemyData>().BigCoinCount);         

                Destroy(obj.gameObject);
                objectsToDestroy.Add(obj.gameObject);
            }
        }

        // 5. 统一销毁被圈中的物体，并从ItemGenerator的列表中移除它们
        foreach (GameObject objToDestroy in objectsToDestroy)
        {
            // 从ItemGenerator的列表中移除对应的Transform
            // 我们需要找到它的Transform来移除
            // Transform transformToRemove = objToDestroy.transform;
            // ItemGenerator.Instance.spawnedTransforms.Remove(transformToRemove);

            // 销毁GameObject
            Debug.Log("Destroy" + objToDestroy.GetType());
            Destroy(objToDestroy);

            int level = TechLevelManager.Instance.CurrentTechLevel;
            float n;
            if (level >= 6)
            {
                n = 0.01f * level;
                if (Random.value <= n)
                    CoinManager._instance.CreateDeadItem(objToDestroy.transform.position);
                n = 0;
            }
        }
    }
    private IEnumerator DelayedDestroy(Transform objTransform, float delay)
    {
        if (objTransform != null)
        {
            yield return new WaitForSeconds(delay);

            // 二次检查对象是否仍然存在
            if (objTransform != null)
            {
                Destroy(objTransform.gameObject);
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
        float rs = r.x * s.y - r.y * s.x;
        if (Mathf.Approximately(rs, 0f)) return false;
        Vector2 qp = b1 - a1;
        float t = (qp.x * s.y - qp.y * s.x) / rs;
        float u = (qp.x * r.y - qp.y * r.x) / rs;
        return t > 0f && t < 1f && u > 0f && u < 1f;
    }

    float CalculateArea(List<Vector3> pts)
    {
        int cnt = pts.Count;
        float area = 0f;
        for (int i = 0, j = cnt - 1; i < cnt; j = i++)
            area += pts[j].x * pts[i].y - pts[i].x * pts[j].y;
        return Mathf.Abs(area) * 0.5f;
    }

    public void StarExplode(Vector3 position)
    {
        GameObject fx = Instantiate(FlashX, position, Quaternion.identity);
    }
}