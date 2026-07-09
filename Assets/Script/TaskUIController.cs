using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIController : MonoBehaviour
{
    public GameObject TaskContainer;

    public GameObject ATaskPanel;

    public float taskDuration;

    public TMP_Text taskTitle;

    public TMP_Text taskContent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAndShowTaskPanelUI(float duration, string title, string content)
    {
        taskDuration = duration;

        var temp = Instantiate(ATaskPanel, TaskContainer.transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(TaskContainer.GetComponent<RectTransform>());

        temp.transform.Find("TaskTitle").GetComponent<TMP_Text>().text = title;
        temp.transform.Find("TaskContent").GetComponent<TMP_Text>().text =  "> "+ content;
        //duration
    }

}
