using UnityEditor;
using UnityEngine;

public class ResetStoryEditor
{
    [MenuItem("Tools/剧情调试/重置 MyStory 剧情记录")]
    public static void ResetMyStoryPlayed()
    {
        string storyFileName = "MyStory"; // 对应 Resources 中的文件名（不带后缀）

        PlayerPrefs.DeleteKey("Story_Played_" + storyFileName);

        Debug.Log("已重置剧情播放记录: " + storyFileName);
    }
}