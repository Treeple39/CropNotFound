// 文件名: StoryData.cs
using System;
using System.Collections.Generic;

// 这个文件不需要挂载到任何GameObject上，它只是定义数据结构。
namespace CustomStorySystem
{
    // 对应JSON文件中的每一行
    [Serializable]
    public class StoryLine
    {
        public int Key;
        public string ContentSpeaker;
        public string Content;
        public string NextContent;
        // 以下字段可以根据你的Excel表格增删
        public string Cha1Action;
        public float CoordinateX1;
        public string Cha1ImageSource;
        public string Cha2Action;
        public float CoordinateX2;
        public string Cha2ImageSource;
        // 更多字段...
        public string BackgroundImagePath;
        public string BackgroundAudioPath;
    }

    // 对应整个JSON文件的根对象
    [Serializable]
    public class Story
    {
        public List<StoryLine> storyData;
    }
}