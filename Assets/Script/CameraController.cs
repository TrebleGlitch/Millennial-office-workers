using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras; // 0: Left, 1: Center, 2: Right ; 3 back
    private int currentIndex = 1; // 初始在中间

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchCamera(currentIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SwitchCamera(currentIndex + 1);
        }
    }

    void SwitchCamera(int newIndex)
    {
        // 降低当前相机的优先级
        cameras[currentIndex].Priority = 10;

        if(newIndex < 0)
            currentIndex = cameras.Length - 1;
        else if(newIndex >= cameras.Length)
            currentIndex = 0;
        else
            currentIndex = newIndex;

        // 提高新相机的优先级，CinemachineBrain会自动进行平滑过渡
        cameras[currentIndex].Priority = 11;
    }
}