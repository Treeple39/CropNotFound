// 文件名: JsonHelper.cs
using UnityEngine;
using CustomStorySystem;
using System.Collections.Generic;

// 这是一个静态工具类，也不需要挂载
public static class JsonHelper
{
    public static Dictionary<int, StoryLine> LoadStory(TextAsset jsonFile)
    {
        // 从TextAsset读取JSON字符串
        Story storyWrapper = JsonUtility.FromJson<Story>(jsonFile.text);

        if (storyWrapper == null || storyWrapper.storyData == null)
        {
            Debug.LogError("JSON解析失败! 请检查 " + jsonFile.name + " 的格式。");
            return null;
        }

        // 将List转换为Dictionary，用Key作为键，方便快速查找
        Dictionary<int, StoryLine> storyDict = new Dictionary<int, StoryLine>();
        foreach (var line in storyWrapper.storyData)
        {
            storyDict[line.Key] = line;
        }
        return storyDict;
    }
}