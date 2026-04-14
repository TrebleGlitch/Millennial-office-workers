using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPlaceController : MonoBehaviour
{
    private Color originalColor;
    private MeshRenderer meshRenderer;

    [HideInInspector] public Vector3 originalPosition; // 记录初始位置，用于放回
    [HideInInspector] public Transform originalParent; // 记录初始父节点

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        originalPosition = transform.position;
        originalParent = transform.parent;
    }

    public void SetHighlight(bool isHighlighted)
    {
        // 简单的原型期高亮方案
        meshRenderer.material.color = isHighlighted ? Color.yellow : originalColor;
    }

    [Header("引用")]
    public TaskConsoleController taskConsole; // 拖入你的任务控制器

    // 当物品被放到这里时，调用这个方法进行判断
    public void ExecuteJudgement(InteractableItem item)
    {
        // 1. 如果这不是一个任务目标物品（比如你把印章丢进来了），直接跳过
        if (!item.isTaskTarget) return;

        // 2. 获取当前正在进行的任务的“标准答案”
        // 注意：你需要确保 TaskConsoleController 里有个方法能获取当前任务
        string target = taskConsole.GetCurrentTaskTargetSequence();

        Debug.Log($"<color=white>判定开始：标准答案[{target}] | 物品暗号[{item.currentSequence}]</color>");

        // 3. 核心方案A逻辑：字符串比对
        if (item.currentSequence == target)
        {
            Debug.Log("<color=green>【任务完成！】顺序和步骤完全正确！</color>");
            // 这里可以调用加分逻辑
        }
        else
        {
            Debug.Log("<color=red>【任务失败】暗号对不上，顺序错了或漏了步骤。</color>");
        }

        // 4. 处理完后，清除物品的暗号（防止下次重复判断）
        item.currentSequence = "";
    }
}
