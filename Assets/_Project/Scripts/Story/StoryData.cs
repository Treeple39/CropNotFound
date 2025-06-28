// 文件名: StoryData.cs
// 作用: 定义剧情文件的数据结构。

namespace SimpleStory
{
    // 这个是JSON文件中每一行对话对应的类
    // [System.Serializable] 用于让Unity的JsonUtility能够处理它
    [System.Serializable]
    public class StoryLine
    {
        // 基础内容
        public string Speaker;
        public string Content;
        public string Avatar;     // 头像资源路径

        // 增强功能
        public string Character;       // 角色立绘指令 (例如: "char1:Characters/Alice_Appear")
        public string Action;          // 动作指令 (例如: "char1:shake,char2:hide")
        public string BackgroundImage; // 背景图资源路径
        public string BackgroundMusic; // 背景音乐资源路径
        public string DialogueSound;   // 对话音效资源路径
        
        // 流程控制
        public int NextID = -1;
        public string[] Choices;
        public int[] ChoiceNextIDs;
    }

    [System.Serializable]
    public class Story
    {
        public System.Collections.Generic.List<StoryLine> storyLines;
    }
}