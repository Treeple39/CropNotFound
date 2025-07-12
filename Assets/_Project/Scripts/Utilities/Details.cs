using UnityEngine;

[System.Serializable]
public abstract class Details
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string IconPath { get; set; }
    public string Description { get; set; }

    [System.NonSerialized] public Sprite Icon; // 加载后填充，不参与序列化
}