using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果你用的是旧版Text，请换成 UnityEngine.UI

public class TaskUI : MonoBehaviour
{
    public Image iconImage;
    public Image pieChartTimer; // 饼图Image
    public TextMeshProUGUI scoreText;

    private float maxTime;
    private float currentTime;
    private bool isActive = false;

    // 初始化任务UI
    public void Setup(TaskData data)
    {
        iconImage.sprite = data.itemIcon;
        scoreText.text = "+" + data.score.ToString();
        maxTime = data.duration;
        currentTime = maxTime;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        // 倒计时逻辑
        currentTime -= Time.deltaTime;

        // 更新饼图 (FillAmount 范围是 0 到 1)
        pieChartTimer.fillAmount = currentTime / maxTime;

        if (currentTime <= 0)
        {
            TaskFailed();
        }
    }

    void TaskFailed()
    {
        isActive = false;
        Debug.Log("任务超时失败！扣除工资！");
        // TODO: 触发TaskManager里的失败扣分逻辑
        Destroy(gameObject); // 销毁当前UI条目
    }

    // 当玩家成功完成任务时调用
    public void TaskCompleted()
    {
        isActive = false;
        Debug.Log("任务完成！获得分数！");
        Destroy(gameObject);
    }
}