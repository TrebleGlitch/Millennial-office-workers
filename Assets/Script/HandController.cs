using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public float handMoveSpeed = 5f;

    private InteractableItem leftHeldItem = null;
    private InteractableItem rightHeldItem = null;

    private InteractableItem currentHoveredItem = null;

    // 记录双手的初始局部位置，抓完东西要收回来
    private Vector3 leftHandBasePos;
    private Vector3 rightHandBasePos;

    void Start()
    {
        leftHandBasePos = leftHand.localPosition;
        rightHandBasePos = rightHand.localPosition;
    }

    void Update()
    {
        HandleMouseTracking();
        HandleInteraction();
    }

    void HandleMouseTracking()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 1. 手部光枪瞄准逻辑 (LookAt)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            leftHand.LookAt(hit.point);
            rightHand.LookAt(hit.point);

            // 2. 高亮逻辑检测
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != currentHoveredItem)
            {
                if (currentHoveredItem != null) currentHoveredItem.SetHighlight(false);
                currentHoveredItem = item;
                if (currentHoveredItem != null) currentHoveredItem.SetHighlight(true);
            }
        }
        else
        {
            if (currentHoveredItem != null)
            {
                currentHoveredItem.SetHighlight(false);
                currentHoveredItem = null;
            }
        }
    }

    void HandleInteraction()
    {
        // 左手抓取/放回
        if (Input.GetMouseButtonDown(0))
        {
            ProcessHandAction(leftHand, ref leftHeldItem, leftHandBasePos);
        }
        // 右手抓取/放回
        else if (Input.GetMouseButtonDown(1))
        {
            ProcessHandAction(rightHand, ref rightHeldItem, rightHandBasePos);
        }
    }

    void ProcessHandAction(Transform hand, ref InteractableItem heldItem, Vector3 baseLocalPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 如果手里为空，且瞄准了物品，执行抓取
            if (heldItem == null && currentHoveredItem != null)
            {
                StartCoroutine(GrabRoutine(hand, currentHoveredItem, baseLocalPos, isLeft: hand == leftHand));
            }
            // 如果手里有物品，且瞄准的是该物品的"空位"（可以利用标签或碰撞体判断，这里用距离简化演示）
            else if (heldItem != null)
            {
                float distToSlot = Vector3.Distance(hit.point, heldItem.originalPosition);
                if (distToSlot < 1.0f) // 射线打到了原位附近
                {
                    StartCoroutine(ReturnRoutine(hand, heldItem, baseLocalPos));
                    // 清空手里引用的逻辑在协程中完成
                }
            }
        }
    }

    // 抓取动画协程：手伸过去 -> 成为父节点 -> 手缩回来
    IEnumerator GrabRoutine(Transform hand, InteractableItem itemToGrab, Vector3 basePos, bool isLeft)
    {
        if (isLeft) leftHeldItem = itemToGrab; else rightHeldItem = itemToGrab;

        Vector3 targetPos = itemToGrab.transform.position;

        // 伸出
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }

        // 抓住
        itemToGrab.transform.SetParent(hand);
        itemToGrab.transform.localPosition = Vector3.forward * 0.5f; // 根据你的球体大小微调位置
        itemToGrab.GetComponent<Rigidbody>().isKinematic = true; // 假设你有刚体，抓取时关闭物理

        // 缩回
        while (Vector3.Distance(hand.localPosition, basePos) > 0.01f)
        {
            hand.localPosition = Vector3.MoveTowards(hand.localPosition, basePos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }
    }

    // 放回动画协程：手伸过去 -> 取消父节点 -> 手缩回来
    IEnumerator ReturnRoutine(Transform hand, InteractableItem itemToReturn, Vector3 basePos)
    {
        Vector3 targetPos = itemToReturn.originalPosition;

        // 伸出
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }

        // 放下
        itemToReturn.transform.SetParent(itemToReturn.originalParent);
        itemToReturn.transform.position = itemToReturn.originalPosition;
        itemToReturn.GetComponent<Rigidbody>().isKinematic = false;

        // 清空手里引用
        if (hand == leftHand) leftHeldItem = null; else rightHeldItem = null;

        // 缩回
        while (Vector3.Distance(hand.localPosition, basePos) > 0.01f)
        {
            hand.localPosition = Vector3.MoveTowards(hand.localPosition, basePos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }
    }
}