using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Batch2DImages : MonoBehaviour
{
    [Tooltip("需要合并的2D图片列表")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("整体透明度(0-1)")]
    [Range(0, 1)]
    public float globalAlpha = 1f;

    [Tooltip("是否每帧更新透明度")]
    public bool updateAlphaPerFrame = false;

    private Mesh combinedMesh;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color combinedColor = Color.white;

    void Start()
    {
        CombineSprites();
    }

    void Update()
    {
        if (updateAlphaPerFrame)
        {
            UpdateAlpha(globalAlpha);
        }
    }

    /// <summary>
    /// 合并所有Sprite为一个网格
    /// </summary>
    public void CombineSprites()
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("没有可合并的Sprite");
            return;
        }

        // 创建合并网格
        combinedMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        int vertexOffset = 0;

        // 创建一张大纹理
        Texture2D atlasTexture = new Texture2D(2048, 2048);
        Rect[] packingResults = atlasTexture.PackTextures(GetTexturesFromSprites(), 0, 2048);

        // 为每个Sprite创建网格数据
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] == null) continue;

            Rect rect = packingResults[i];
            Sprite sprite = sprites[i];

            // 添加4个顶点
            vertices.Add(new Vector3(rect.xMin * 2 - 1, rect.yMin * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMax * 2 - 1, rect.yMin * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMax * 2 - 1, rect.yMax * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMin * 2 - 1, rect.yMax * 2 - 1, 0));

            // 添加UV(使用原始Sprite的UV)
            uv.Add(sprite.uv[0]);
            uv.Add(sprite.uv[1]);
            uv.Add(sprite.uv[2]);
            uv.Add(sprite.uv[3]);

            // 添加顶点颜色(初始为白色，透明度由全局控制)
            colors.Add(Color.white);
            colors.Add(Color.white);
            colors.Add(Color.white);
            colors.Add(Color.white);

            // 添加三角形
            triangles.Add(vertexOffset + 0);
            triangles.Add(vertexOffset + 1);
            triangles.Add(vertexOffset + 2);
            triangles.Add(vertexOffset + 0);
            triangles.Add(vertexOffset + 2);
            triangles.Add(vertexOffset + 3);

            vertexOffset += 4;
        }

        // 设置网格数据
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uv.ToArray();
        combinedMesh.colors = colors.ToArray();
        combinedMesh.triangles = triangles.ToArray();

        // 设置MeshFilter
        GetComponent<MeshFilter>().mesh = combinedMesh;

        // 使用默认的Sprite渲染材质
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        // 初始化MaterialPropertyBlock用于高效修改材质属性
        materialPropertyBlock = new MaterialPropertyBlock();
        GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetTexture("_MainTex", atlasTexture);
        materialPropertyBlock.SetColor("_Color", combinedColor);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);

        // 设置初始透明度
        UpdateAlpha(globalAlpha);
    }

    /// <summary>
    /// 从Sprite列表获取Texture2D数组
    /// </summary>
    private Texture2D[] GetTexturesFromSprites()
    {
        List<Texture2D> textures = new List<Texture2D>();
        foreach (var sprite in sprites)
        {
            if (sprite != null)
            {
                textures.Add(sprite.texture);
            }
        }
        return textures.ToArray();
    }

    /// <summary>
    /// 更新整体透明度
    /// </summary>
    public void UpdateAlpha(float alpha)
    {
        globalAlpha = Mathf.Clamp01(alpha);

        if (combinedMesh != null && materialPropertyBlock != null)
        {
            // 更新顶点颜色alpha值
            Color[] colors = combinedMesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = globalAlpha;
            }
            combinedMesh.colors = colors;

            // 同时设置材质颜色(可选)
            combinedColor.a = globalAlpha;
            GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", combinedColor);
            GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
        }
    }

    /// <summary>
    /// 添加新的Sprite到批处理中(需要重新合并)
    /// </summary>
    public void AddSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            sprites.Add(sprite);
            CombineSprites();
        }
    }

    /// <summary>
    /// 清除所有合并的Sprite
    /// </summary>
    public void ClearSprites()
    {
        sprites.Clear();
        if (GetComponent<MeshFilter>().mesh != null)
        {
            Destroy(GetComponent<MeshFilter>().mesh);
        }
        GetComponent<MeshFilter>().mesh = null;
    }
}