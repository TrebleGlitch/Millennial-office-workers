using UnityEngine;

// [System.Serializable] 是必须的，它让这个类能在 Unity 的 Inspector 面板里显示出来
[System.Serializable]
public class TaskData
{
    public string taskName;   // 任务名字（如：盖章、贴票）
    public int scoreReward;   // 完成奖励多少分
    public float duration;    // 任务限制时长（秒）
    [Tooltip("正确操作顺序，例如: Stamp;Sign; (注意分号要统一)")]
    public string requiredSequence;
}