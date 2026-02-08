using System;
using UnityEngine;

[Serializable]
public class ClueData
{
    public string id;          // 唯一ID
    public string title;       // 标题/名字
    [TextArea] public string description;

    public ClueData(string id, string title, string description)
    {
        this.id = id;
        this.title = title;
        this.description = description;
    }
}
