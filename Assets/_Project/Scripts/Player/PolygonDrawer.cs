using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PolygonDrawer : MonoBehaviour
{
    // ★★★ 核心修改 #1：新的动画参数 ★★★
    [Header("外观参数")]
    [Tooltip("如果留空，会自动创建一个支持顶点颜色的透明材质")]
    public Material polygonMaterial;
    [Tooltip("淡入到半透明状态所需的时间")]
    public float fadeInDuration = 2f; 
    [Tooltip("从半透明状态淡出到消失所需的时间")]
    public float fadeOutDuration = 2f;

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (polygonMaterial == null)
        {
            // 使用一个能响应顶点颜色和透明度的可靠着色器
            Shader shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            if (shader == null) shader = Shader.Find("Sprites/Default"); // Sprites/Default也是一个很好的选择
            polygonMaterial = new Material(shader);
        }
        meshRenderer.material = polygonMaterial;
    }

    /// <summary>
    /// 外部调用的主方法，用于绘制多边形并开始闪现动画
    /// </summary>
    /// <param name="points">构成多边形的顶点列表</param>
    public void DrawAndAnimatePulse(List<Vector3> points)
    {
        CreatePolygonMesh(points);
        StartCoroutine(PolygonPulseLifecycle()); // 调用新的生命周期协程
    }

    private void CreatePolygonMesh(List<Vector3> points)
    {
        if (points == null || points.Count < 3) return;

        Vector2[] vertices2D = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++) { vertices2D[i] = points[i]; }
        this.vertices = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);

        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        this.colors = new Color[this.vertices.Length];
        Color transparentWhite = new Color(1, 1, 1, 0);
        for (int i = 0; i < this.colors.Length; i++) { this.colors[i] = transparentWhite; }

        mesh.Clear();
        mesh.vertices = this.vertices;
        mesh.triangles = indices;
        mesh.colors = this.colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// ★★★ 核心修改 #2：实现 Alpha 0 -> 0.5 -> 0 的脉冲生命周期 ★★★
    /// </summary>
    private IEnumerator PolygonPulseLifecycle()
    {
        Color baseColor = Color.white; // 我们的基础颜色是白色
        float timer = 0f;

        // --- 阶段1: Fade In (Alpha 0 -> 0.5) ---
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            // 计算当前进度 (0到1)
            float progress = Mathf.Clamp01(timer / fadeInDuration);
            // 将alpha值从0插值到0.5
            float currentAlpha = Mathf.Lerp(0f, 0.5f, progress);
            
            // 更新所有顶点的颜色
            Color currentColor = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);
            UpdateAllVertexColors(currentColor);

            yield return null;
        }
        
        // 确保达到峰值
        UpdateAllVertexColors(new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f));

        // --- 阶段2: Fade Out (Alpha 0.5 -> 0) ---
        timer = 0f; // 重置计时器
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeOutDuration);
            // 将alpha值从0.5插值到0
            float currentAlpha = Mathf.Lerp(0.5f, 0f, progress);
            
            // 更新所有顶点的颜色
            Color currentColor = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);
            UpdateAllVertexColors(currentColor);

            yield return null;
        }

        // --- 阶段3: 销毁 ---
        Destroy(gameObject);
    }

    /// <summary>
    /// 一个辅助方法，用于更新所有顶点的颜色并应用到网格
    /// </summary>
    private void UpdateAllVertexColors(Color color)
    {
        for (int i = 0; i < this.colors.Length; i++)
        {
            this.colors[i] = color;
        }
        mesh.colors = this.colors;
    }
}


// Triangulator 辅助类保持不变
public class Triangulator
{
    // ... (Triangulator代码无需修改) ...
    private List<Vector2> m_points;
    public Triangulator(Vector2[] points) { m_points = new List<Vector2>(points); }
    public int[] Triangulate()
    {
        List<int> indices = new List<int>();
        int n = m_points.Count;
        if (n < 3) return indices.ToArray();
        int[] V = new int[n];
        if (Area() > 0) { for (int v = 0; v < n; v++) V[v] = v; }
        else { for (int v = 0; v < n; v++) V[v] = (n - 1) - v; }
        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0) return indices.ToArray();
            int u = v; if (nv <= u) u = 0; v = u + 1; if (nv <= v) v = 0;
            int w = v + 1; if (nv <= w) w = 0;
            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u]; b = V[v]; c = V[w];
                indices.Add(a); indices.Add(b); indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }
        indices.Reverse();
        return indices.ToArray();
    }
    private float Area()
    {
        int n = m_points.Count; float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        { A += m_points[p].x * m_points[q].y - m_points[q].x * m_points[p].y; }
        return (A * 0.5f);
    }
    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p; Vector2 A = m_points[V[u]], B = m_points[V[v]], C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x)))) return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w)) continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P)) return false;
        }
        return true;
    }
    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;
        ax = C.x - B.x; ay = C.y - B.y; bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y; apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y; cpx = P.x - C.x; cpy = P.y - C.y;
        aCROSSbp = ax * bpy - ay * bpx; cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;
        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}