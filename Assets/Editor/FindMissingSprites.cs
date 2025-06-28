// 放到 Assets/Editor/FindNullSpriteRenderers.cs
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;

public static class FindNullSpriteRenderers
{
    [MenuItem("Tools/🔍 全项目查空 SpriteRenderer(Assets+Packages)")]
    public static void ScanAll()
    {
        int count = 0;

        // 1) 扫场景里
        foreach (var sr in Object.FindObjectsOfType<SpriteRenderer>(true))
        {
            if (sr.sprite == null)
            {
                Debug.LogWarning($"[Scene] 空 SpriteRenderer 在: {sr.gameObject.name}", sr.gameObject);
                count++;
            }
        }

        // 2) 扫 Assets 里的所有 Prefab
        var allPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[]{ "Assets" });
        foreach (var guid in allPrefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);
            foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (sr.sprite == null)
                {
                    Debug.LogWarning($"[Assets Prefab] {path} -> {sr.gameObject.name}");
                    count++;
                }
            }
            PrefabUtility.UnloadPrefabContents(root);
        }

        // 3) 扫 Packages 里的所有 Prefab
        var packageList = Client.List(true, true);
        while (!packageList.IsCompleted) { }
        foreach (var pkg in packageList.Result)
        {
            if (string.IsNullOrEmpty(pkg.assetPath)) continue;
            var pkgPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[]{ pkg.assetPath });
            foreach (var guid in pkgPrefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var root = PrefabUtility.LoadPrefabContents(path);
                foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (sr.sprite == null)
                    {
                        Debug.LogWarning($"[Package Prefab] {path} -> {sr.gameObject.name}");
                        count++;
                    }
                }
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        EditorUtility.DisplayDialog("查找完成", $"共发现 {count} 处空 SpriteRenderer，详情请看 Console", "OK");
    }
}