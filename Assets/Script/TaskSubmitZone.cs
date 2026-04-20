using UnityEngine;

public class TaskSubmitZone : MonoBehaviour
{
    [Header("核心引用")]
    [Tooltip("拖入挂载了 TaskConsoleController 的 GameManager")]
    public TaskConsoleController taskConsole;

    // ==========================================
    // 触发方式 1：物理碰撞触发 (推荐)
    // 如果你的收件箱带有一个 Collider (勾选了 Is Trigger) 
    // 并且玩家是通过松开手让物体掉进去的，用这个方法最方便。
    // ==========================================
 
    // ==========================================
    // 触发方式 2：代码直接调用
    // 如果玩家是通过鼠标点击收件箱来“递交”手里的物品，
    // 可以让 HandController 调用这个公开方法。
    // ==========================================
    public void ReceiveItemFromHand(GameObject itemObj)
    {
        TrySubmitItem(itemObj);
    }

    // --- 核心验证逻辑 ---
    private void TrySubmitItem(GameObject itemObj)
    {
        // 1. 验证标签：是成品吗？
        if (itemObj.CompareTag("WorkPlace"))
        {
            Debug.Log("<color=cyan>【收件箱】检测到成品 (WorkPlace)！正在结算...</color>");

            // 2. 通知控制台加分
            if (taskConsole != null)
            {
                taskConsole.CompleteCurrentTask();
            }
            else
            {
                Debug.LogWarning("收件箱没有连接 TaskConsoleController，无法加分！");
            }

            // 3. 销毁成品
            Destroy(itemObj);
        }
        else
        {
            // 如果丢进来的不是成品（比如是个空盘子或者没盖章的发票）
            Debug.Log($"<color=yellow>【收件箱】拒收！物品 [{itemObj.name}] 没有 WorkPlace 标签。</color>");
        }
    }
}