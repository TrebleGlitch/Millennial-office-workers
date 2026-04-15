using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public float handMoveSpeed = 5f;

    [Header("Work Place Area")]
    public Transform workPlaceAreaCenter;
    public WorkPlaceController workPlaceController;
    public float placeToWorkPlaceDistance = 1.0f;

    private InteractableItem leftHeldItem = null;
    private InteractableItem rightHeldItem = null;

    [Header("Held Item Visual (Clone + Pool)")]
    public Vector3 heldLocalPositionOffset = new Vector3(0f, 0f, 0.5f);
    public Vector3 heldLocalEulerOffset = Vector3.zero;
    public Vector3 heldLocalScale = Vector3.one;

    private GameObject leftHeldVisual = null;
    private int leftHeldVisualKey = 0;
    private GameObject rightHeldVisual = null;
    private int rightHeldVisualKey = 0;

    private readonly Dictionary<int, Stack<GameObject>> pooledVisuals = new Dictionary<int, Stack<GameObject>>();

    private InteractableItem currentHoveredItem = null;

    // ��¼˫�ֵĳ�ʼ�ֲ�λ�ã�ץ�궫��Ҫ�ջ���
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

        // 1. �ֲ���ǹ��׼�߼� (LookAt)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            //手部跟随光标
            //leftHand.LookAt(hit.point);
            //rightHand.LookAt(hit.point);

            // Interactable
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != currentHoveredItem)
            {
                if (currentHoveredItem != null) currentHoveredItem.SetHighlight(false);
                currentHoveredItem = item;
                if (currentHoveredItem != null) currentHoveredItem.SetHighlight(true);
            }

            // WorkPlace
            
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
        // ����ץȡ/�Ż�
        if (Input.GetMouseButtonDown(0))
        {
            ProcessHandAction(leftHand, ref leftHeldItem, leftHandBasePos);
        }
        // ����ץȡ/�Ż�
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
            // 手是空的
            if (heldItem == null && currentHoveredItem != null)
            {
                StartCoroutine(GrabRoutine(hand, currentHoveredItem, baseLocalPos, isLeft: hand == leftHand));
            }
            // 手上有东西
            else if (heldItem != null)
            {
                // 优先放到 WorkPlaceAreaCenter（同样用鼠标射线点击 + 距离判定）
                if (workPlaceAreaCenter != null)
                {
                    float distToWorkPlace = Vector3.Distance(hit.point, workPlaceAreaCenter.position);
                    if (distToWorkPlace < placeToWorkPlaceDistance)
                    {
                        StartCoroutine(PlaceToWorkPlaceRoutine(hand, baseLocalPos, isLeft: hand == leftHand));
                        return;
                    }
                }

                float distToSlot = Vector3.Distance(hit.point, heldItem.originalPosition);
                if (distToSlot < 1.0f)
                {
                    StartCoroutine(ReturnRoutine(hand, heldItem, baseLocalPos));
                }
            }
        }
    }
    IEnumerator PlaceToRightTableSubmit()
    {
        // bin
        // submit

        yield return null;
    }
    IEnumerator PlaceToWorkPlaceRoutine(Transform hand, Vector3 basePos, bool isLeft)
    {
        if (workPlaceAreaCenter == null) yield return null;

        GameObject visual = isLeft ? leftHeldVisual : rightHeldVisual;
        InteractableItem held = isLeft ? leftHeldItem : rightHeldItem;

        if (visual == null || held == null) yield return null;

        Vector3 targetPos = workPlaceAreaCenter.position;

        // 手移动到工作区中心
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }

        // 放置：把手持替身从手上解绑并移动到中心
        if (workPlaceAreaCenter.Find("pan(Clone)") == null && visual.GetComponent<InteractableItem>().toolActionName != "Pan")
        {
            // 无盘子 不能放
            yield return null;
        }
        else
        {
            if (visual.GetComponent<InteractableItem>().toolActionName == "Pan")
            {
                if (workPlaceAreaCenter.Find("pan(Clone)") != null)
                {
                    // xiao hui pan on the hand
                    yield return null;
                }

                visual.transform.position = workPlaceAreaCenter.position;
                visual.transform.rotation = workPlaceAreaCenter.rotation;
                visual.transform.SetParent(workPlaceAreaCenter);
            }
            else
            {
                visual.transform.SetParent(workPlaceAreaCenter.Find("pan(Clone)").transform);
                visual.transform.position = workPlaceAreaCenter.position;
                visual.transform.rotation = workPlaceAreaCenter.rotation;

                // 记录 string
                workPlaceController.ApplyItemPlaceAction(visual.GetComponent<InteractableItem>().toolActionName);
            }    
        }

        // 清空手持状态
        if (isLeft)
        {
            leftHeldVisual = null;
            leftHeldVisualKey = 0;
            leftHeldItem = null;
        }
        else
        {
            rightHeldVisual = null;
            rightHeldVisualKey = 0;
            rightHeldItem = null;
        }

        // 手回到原位
        while (Vector3.Distance(hand.localPosition, basePos) > 0.01f)
        {
            hand.localPosition = Vector3.MoveTowards(hand.localPosition, basePos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }
    }

    int GetPoolKey(InteractableItem source)
    {
        if (source == null) return 0;

        // Prefer shared mesh as a stable "type" key across identical items.
        MeshFilter mf = source.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null) return mf.sharedMesh.GetInstanceID();

        // Fallback: per-instance key (still works, just less pooling reuse).
        return source.gameObject.GetInstanceID();
    }

    GameObject GetPooledClone(InteractableItem source, Transform hand, out int key)
    {
        // 拿到的物品 
        key = GetPoolKey(source);

        // 你池子里有了对应的物品 我就直接拿 不需要生成
        if (key != 0 && pooledVisuals.TryGetValue(key, out Stack<GameObject> stack) && stack.Count > 0)
        {
            GameObject reused = stack.Pop();
            reused.transform.SetParent(hand, worldPositionStays: false);
            reused.transform.localPosition = heldLocalPositionOffset;
            reused.transform.localEulerAngles = heldLocalEulerOffset;
            reused.transform.localScale = heldLocalScale;
            reused.SetActive(true);
            return reused;
        }

        // 如果池子没有 我再执行这里 来生产一个需要的物品
        GameObject targetObj;
        if (source.theGrabedObj == null)
            targetObj = source.gameObject;
        else
            targetObj = source.theGrabedObj;

        GameObject clone = Instantiate(targetObj);
        //clone.name = $"{source.gameObject.name}_HeldVisual";
        clone.transform.SetParent(hand, worldPositionStays: false);
        clone.transform.localPosition = heldLocalPositionOffset;
        clone.transform.localEulerAngles = heldLocalEulerOffset;
        clone.transform.localScale = heldLocalScale;

        // Make sure the visual clone doesn't interfere with raycasts / physics.
        foreach (Collider c in clone.GetComponentsInChildren<Collider>(includeInactive: true))
        {
            c.enabled = false;
        }
        foreach (Rigidbody rb in clone.GetComponentsInChildren<Rigidbody>(includeInactive: true))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        foreach (InteractableItem ii in clone.GetComponentsInChildren<InteractableItem>(includeInactive: true))
        {
            ii.enabled = false;
        }

        return clone;
    }

    void ReturnToPool(int key, GameObject visual)
    {
        if (visual == null) return;

        visual.SetActive(false);
        visual.transform.SetParent(null);

        if (key == 0)
        {
            Destroy(visual);
            return;
        }

        if (!pooledVisuals.TryGetValue(key, out Stack<GameObject> stack))
        {
            stack = new Stack<GameObject>();
            pooledVisuals[key] = stack;
        }

        stack.Push(visual);
    }

    // ץȡ����Э�̣������ȥ -> ��Ϊ���ڵ� -> ��������
    IEnumerator GrabRoutine(Transform hand, InteractableItem itemToGrab, Vector3 basePos, bool isLeft)
    {
        if (isLeft) leftHeldItem = itemToGrab; else rightHeldItem = itemToGrab;

        Vector3 targetPos = itemToGrab.transform.position;

        // 把手移动到目标点
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }


        // if is WORK PLACE S
        if(itemToGrab.gameObject.tag == "WorkPlace")
        {
            itemToGrab.transform.SetParent(hand);
        }
        else
        {
            // ץס
            if (isLeft)
            {
                if (leftHeldVisual != null) ReturnToPool(leftHeldVisualKey, leftHeldVisual);
                leftHeldVisual = GetPooledClone(itemToGrab, hand, out leftHeldVisualKey);
            }
            else
            {
                if (rightHeldVisual != null) ReturnToPool(rightHeldVisualKey, rightHeldVisual);
                rightHeldVisual = GetPooledClone(itemToGrab, hand, out rightHeldVisualKey);
            }
        }
        

        // send a task
        TaskConsoleController.Instance.GenerateRandomTask(); // 

        // ����
        while (Vector3.Distance(hand.localPosition, basePos) > 0.01f)
        {
            hand.localPosition = Vector3.MoveTowards(hand.localPosition, basePos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }
    }

    // �Żض���Э�̣������ȥ -> ȡ�����ڵ� -> ��������
    IEnumerator ReturnRoutine(Transform hand, InteractableItem itemToReturn, Vector3 basePos)
    {
        Vector3 targetPos = itemToReturn.originalPosition;

        // ���
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }

        // ����
        if (hand == leftHand)
        {
            ReturnToPool(leftHeldVisualKey, leftHeldVisual);
            leftHeldVisual = null;
            leftHeldVisualKey = 0;
        }
        else
        {
            ReturnToPool(rightHeldVisualKey, rightHeldVisual);
            rightHeldVisual = null;
            rightHeldVisualKey = 0;
        }

        // �����������
        if (hand == leftHand) leftHeldItem = null; else rightHeldItem = null;

        // ����
        while (Vector3.Distance(hand.localPosition, basePos) > 0.01f)
        {
            hand.localPosition = Vector3.MoveTowards(hand.localPosition, basePos, Time.deltaTime * handMoveSpeed);
            yield return null;
        }
    }
}