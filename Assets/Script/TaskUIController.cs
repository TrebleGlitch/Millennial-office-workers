using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskUIController : MonoBehaviour
{
    public GameObject TaskContainer;

    public GameObject ATaskPanel;

    public float taskduration;

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
        taskduration = duration;

        //taskTitle.text = title;

        //taskContent.text = content;


        var temp = Instantiate(ATaskPanel, TaskContainer.transform);

        temp.transform.Find("TaskTitle").GetComponent<TMP_Text>().text = title;
        temp.transform.Find("TaskContent").GetComponent<TMP_Text>().text =  "> "+ content;
        //duration
    }

}
