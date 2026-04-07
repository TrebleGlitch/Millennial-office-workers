using System.Collections.Generic;
using UnityEngine;

public class TaskConsoleController : MonoBehaviour
{
    [Header("任务池配置")]
    [Tooltip("在这里配置所有可能随机出现的任务模板")]
    public List<TaskData> taskPool;

    [Header("生成设置")]
    [Tooltip("每次生成新任务的间隔时间（秒）")]
    public float spawnInterval = 5f;

    // 内部计时器
    private float currentTimer;
    // 用于给生成的任务标号，方便在控制台查看
    private int taskCounter = 0;

    void Start()
    {
        Debug.Log("<color=cyan>=== 动态任务调度中心已启动 ===</color>");

        if (taskPool == null || taskPool.Count == 0)
        {
            Debug.LogError("警告：任务池为空！请先在 Inspector 中添加任务。");
            return;
        }

        Debug.Log($"任务池加载完毕，共 {taskPool.Count} 种任务。将每隔 {spawnInterval} 秒下发一个新任务。");

        // 初始化计时器
        currentTimer = spawnInterval;
    }

    void Update()
    {
        // 容错处理：如果没有配置任务，则不跑倒计时
        if (taskPool == null || taskPool.Count == 0) return;

        // 倒计时核心逻辑
        currentTimer -= Time.deltaTime;

        // 当计时器归零或小于零时，触发生成逻辑
        if (currentTimer <= 0f)
        {
            GenerateRandomTask();
            currentTimer = spawnInterval; // 重置计时器，准备下一次生成
        }
    }

    void GenerateRandomTask()
    {
        // 1. 核心方法：Random.Range(min, max)。
        // 注意：对于整数，Random.Range 是包含最小值，不包含最大值的。
        // 所以 Random.Range(0, taskPool.Count) 刚好能安全地覆盖 List 的所有索引 (0 到 Count-1)
        int randomIndex = Random.Range(0, taskPool.Count);

        // 2. 取出对应的任务数据
        TaskData selectedTask = taskPool[randomIndex];

        taskCounter++;

        // 3. 打印到控制台 (使用了富文本颜色标签高亮显示)
        Debug.Log($"<color=yellow>[叮！新任务到达 - 编号 #{taskCounter}]</color> 目标: <b>{selectedTask.taskName}</b> | 限时: {selectedTask.duration}秒 | 奖励: {selectedTask.scoreReward}分");
    }
}