// 文件名: JsonHelper.cs
// 作用: 一个简单的静态类，用于从TextAsset加载剧情数据。

using UnityEngine;

namespace SimpleStory
{
    public static class JsonHelper
    {
        public static System.Collections.Generic.List<StoryLine> LoadStory(TextAsset jsonFile)
        {
            Story storyWrapper = JsonUtility.FromJson<Story>(jsonFile.text);
            if (storyWrapper == null || storyWrapper.storyLines == null)
            {
                Debug.LogError("无法解析JSON文件: " + jsonFile.name + "。请检查格式是否正确，特别是外层的 'storyLines' 对象。");
                return new System.Collections.Generic.List<StoryLine>();
            }
            return storyWrapper.storyLines;
        }
    }
}