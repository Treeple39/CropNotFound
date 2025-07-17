using UnityEngine;

public static class ResourceManager
{
    public static T Load<T>(string resourceName) where T : UnityEngine.Object
    {
        return Resources.Load<T>(resourceName);
    }

    public static Sprite LoadSprite(string resourceName)
    {
        return Load<Sprite>("Sprites/" + resourceName);
    }

    public static Sprite LoadRandomSprite(string folderPath)
    {
        //加载文件夹下所有Sprite
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/" + folderPath);

        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogError($"No sprites found at path Sprites/{folderPath}");
            return null;
        }

        //随机选择一个
        return allSprites[Random.Range(0, allSprites.Length)];
    }

    public static GameObject LoadPrefab(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Try to load empty Prefab");
            return null;
        }

        GameObject prefab = Load<GameObject>("Prefabs/" + path);
        if (prefab == null)
            Debug.LogError($"Can not find Prefab: {path}");

        return prefab;
    }

    public static string LoadStory(string resourceName)
    {
        return Load<TextAsset>("Story/" + resourceName).text;
    }
}