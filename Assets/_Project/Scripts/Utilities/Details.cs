using UnityEngine;

[System.Serializable]
public abstract class Details
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string IconPath { get; set; }
    public string Description { get; set; }

    [System.NonSerialized] public Sprite Icon; // ���غ���䣬���������л�
}