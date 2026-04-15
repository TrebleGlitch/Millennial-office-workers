using System.Collections.Generic;
using UnityEngine;

public class TaskConsoleController : MonoBehaviour
{
    private static TaskConsoleController _instance;

    public static TaskConsoleController Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }

    [Header("任务池配置")]
    public List<TaskData> taskPool;

    [Header("随机生成设置")]
    [Tooltip("任务生成的最小时间间隔（秒）")]
    public float minInterval = 2f;
    [Tooltip("任务生成的最大时间间隔（秒）")]
    public float maxInterval = 8f;

    private float currentTimer;
    private float nextInterval; // 存储当前这一轮随机出来的目标间隔
    private int taskCounter = 0;

    TaskData currentActiveTask;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Debug.Log("<color=cyan>=== 动态随机任务调度中心已启动 ===</color>");

        if (taskPool == null || taskPool.Count == 0)
        {
            Debug.LogError("任务池为空，请先在 Inspector 中配置任务！");
            return;
        }

        // 初始第一轮：随机一个间隔时间
        SetNextRandomInterval();
    }

    void Update()
    {
        //if (taskPool == null || taskPool.Count == 0) return;

        //// 计时器递减
        ////currentTimer -= Time.deltaTime;

        //// 到达随机目标时间
        //if (currentTimer <= 0f)
        //{
        //    GenerateRandomTask();
        //    // 关键改动：任务生成后，立即重新计算下一轮的随机间隔
        //    SetNextRandomInterval();
        //}
    }

    /// <summary>
    /// 计算并设置下一个随机生成的间隔时间
    /// </summary>
    void SetNextRandomInterval()
    {
        // Random.Range 处理 float 时是包含最大值的 [min, max]
        nextInterval = Random.Range(minInterval, maxInterval);
        currentTimer = nextInterval;

        Debug.Log($"<color=grey>[系统预报] 下一个任务将在 {nextInterval:F2} 秒后到达...</color>");
    }

    public void GenerateRandomTask()
    {
        int randomIndex = Random.Range(0, taskPool.Count);
        TaskData selectedTask = taskPool[randomIndex];

        taskCounter++;

        Debug.Log($"<color=yellow>[叮！新任务到达 - 编号 #{taskCounter}]</color> 目标: <b>{selectedTask.taskName}</b> | 限时: {selectedTask.duration}秒 | 奖励: {selectedTask.scoreReward}分");
    }

    // 在 TaskConsoleController.cs 中
    public string GetCurrentTaskTargetSequence()
    {
        // 返回你当前随机生成的那个任务的 requiredSequence
        return currentActiveTask.requiredSequence;
    }
}