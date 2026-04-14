using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    private Color originalColor;
    private MeshRenderer meshRenderer;

    [HideInInspector] public Vector3 originalPosition; // 记录初始位置，用于放回
    [HideInInspector] public Transform originalParent; // 记录初始父节点

    public GameObject theGrabedObj;

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

    [Header("任务记录")]
    public string currentSequence = ""; // 记录操作顺序
    public bool isTaskTarget = false;   // 标记这是否是需要提交的票据（比如发票）
    public string toolActionName = "";  // 如果是工具（如印章），它产生的动作名（如 "Stamp;"）

    // 新增一个方法：被处理时调用
    public void ApplyAction(string actionName)
    {
        currentSequence += actionName;
        Debug.Log($"{name} 现在的暗号是: {currentSequence}");
    }
}