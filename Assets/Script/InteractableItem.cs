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

    // workplace
    [Header("行动名称")]
    public string toolActionName = "";  // 如果是工具（如印章），它产生的动作名（如 "Stamp;"）
}