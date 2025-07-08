using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Batch2DImages : MonoBehaviour
{
    [Tooltip("��Ҫ�ϲ���2DͼƬ�б�")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("����͸����(0-1)")]
    [Range(0, 1)]
    public float globalAlpha = 1f;

    [Tooltip("�Ƿ�ÿ֡����͸����")]
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
    /// �ϲ�����SpriteΪһ������
    /// </summary>
    public void CombineSprites()
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("û�пɺϲ���Sprite");
            return;
        }

        // �����ϲ�����
        combinedMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        int vertexOffset = 0;

        // ����һ�Ŵ�����
        Texture2D atlasTexture = new Texture2D(2048, 2048);
        Rect[] packingResults = atlasTexture.PackTextures(GetTexturesFromSprites(), 0, 2048);

        // Ϊÿ��Sprite������������
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] == null) continue;

            Rect rect = packingResults[i];
            Sprite sprite = sprites[i];

            // ���4������
            vertices.Add(new Vector3(rect.xMin * 2 - 1, rect.yMin * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMax * 2 - 1, rect.yMin * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMax * 2 - 1, rect.yMax * 2 - 1, 0));
            vertices.Add(new Vector3(rect.xMin * 2 - 1, rect.yMax * 2 - 1, 0));

            // ���UV(ʹ��ԭʼSprite��UV)
            uv.Add(sprite.uv[0]);
            uv.Add(sprite.uv[1]);
            uv.Add(sprite.uv[2]);
            uv.Add(sprite.uv[3]);

            // ��Ӷ�����ɫ(��ʼΪ��ɫ��͸������ȫ�ֿ���)
            colors.Add(Color.white);
            colors.Add(Color.white);
            colors.Add(Color.white);
            colors.Add(Color.white);

            // ���������
            triangles.Add(vertexOffset + 0);
            triangles.Add(vertexOffset + 1);
            triangles.Add(vertexOffset + 2);
            triangles.Add(vertexOffset + 0);
            triangles.Add(vertexOffset + 2);
            triangles.Add(vertexOffset + 3);

            vertexOffset += 4;
        }

        // ������������
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uv.ToArray();
        combinedMesh.colors = colors.ToArray();
        combinedMesh.triangles = triangles.ToArray();

        // ����MeshFilter
        GetComponent<MeshFilter>().mesh = combinedMesh;

        // ʹ��Ĭ�ϵ�Sprite��Ⱦ����
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        // ��ʼ��MaterialPropertyBlock���ڸ�Ч�޸Ĳ�������
        materialPropertyBlock = new MaterialPropertyBlock();
        GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetTexture("_MainTex", atlasTexture);
        materialPropertyBlock.SetColor("_Color", combinedColor);
        GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);

        // ���ó�ʼ͸����
        UpdateAlpha(globalAlpha);
    }

    /// <summary>
    /// ��Sprite�б��ȡTexture2D����
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
    /// ��������͸����
    /// </summary>
    public void UpdateAlpha(float alpha)
    {
        globalAlpha = Mathf.Clamp01(alpha);

        if (combinedMesh != null && materialPropertyBlock != null)
        {
            // ���¶�����ɫalphaֵ
            Color[] colors = combinedMesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].a = globalAlpha;
            }
            combinedMesh.colors = colors;

            // ͬʱ���ò�����ɫ(��ѡ)
            combinedColor.a = globalAlpha;
            GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", combinedColor);
            GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
        }
    }

    /// <summary>
    /// ����µ�Sprite����������(��Ҫ���ºϲ�)
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
    /// ������кϲ���Sprite
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