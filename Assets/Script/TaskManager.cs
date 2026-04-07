using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform taskPanelParent; // 左上角的排布容器
    public GameObject taskUIPrefab;   // 任务UI的预制体

    [Header("Task Generation Config")]
    public float spawnInterval = 5f;  // 每隔几秒生成一个新任务
    private float spawnTimer;

    [Header("Task Pool")]
    public List<TaskData> presetTasks; // 在Inspector中配置你的预设任务列表

    // 存储当前正在运行的任务
    private List<TaskUI> activeTasks = new List<TaskUI>();

    void Start()
    {
        spawnTimer = spawnInterval; // 游戏开始马上生成一个，或者等待一轮
    }

    void Update()
    {
        // 定时生成逻辑
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            GenerateRandomTask();
            spawnTimer = spawnInterval; // 重置计时器
        }
    }

    void GenerateRandomTask()
    {
        if (presetTasks.Count == 0) return;

        // 1. 从列表中随机抽取一个任务数据
        int randomIndex = Random.Range(0, presetTasks.Count);
        TaskData selectedData = presetTasks[randomIndex];

        // 2. 实例化 UI 预制体到左上角面板
        GameObject newTaskObj = Instantiate(taskUIPrefab, taskPanelParent);

        // 3. 获取UI组件并注入数据
        TaskUI taskUI = newTaskObj.GetComponent<TaskUI>();
        taskUI.Setup(selectedData);

        // 4. 加入活跃列表（备后续查验完成状态用）
        activeTasks.Add(taskUI);

        Debug.Log($"生成了新任务：抓取 {selectedData.targetItem}，时长 {selectedData.duration}秒");
    }

    // 预留的接口：当玩家抓起物品时，通知TaskManager检查是否完成任务
    public void CheckTaskCompletion(ItemType grabbedItemType)
    {
        // 遍历当前活跃任务，看是否有匹配的
        // (注：这里只是原型逻辑，实际情况可能需要玩家不仅抓起来，还要放到特定位置才算完成)
    }
}