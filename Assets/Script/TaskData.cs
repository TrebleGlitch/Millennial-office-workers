using UnityEngine;

// 定义可交互物品的种类
public enum ItemType
{
    Stamp,      // 印章
    Invoice,    // 发票
    Pen,        // 笔
    Towel       // 毛巾（用来擦手汗）
}

// 预设任务的模板数据
[System.Serializable]
public class TaskData
{
    public string taskName;       // 任务名称（备用）
    public ItemType targetItem;   // 需要抓取的物品
    public float duration;        // 任务时长（秒）
    public int score;             // 完成后的分数奖励
    public Sprite itemIcon;       // UI上显示的图标
}