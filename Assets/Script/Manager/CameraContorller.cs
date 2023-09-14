using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraContorller : MonoBehaviour
{
    [SerializeField]
    private Camera view_cam;        //相机对象

    public float s = 0.001f;        // 鼠标移动速度的缩放因子


    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 last = Vector3.zero;    // 上一帧鼠标位置
    float mouseSpeed = 10;          // 鼠标移动速度
    // Update is called once per frame
    void Update()
    {
        // 使用滚轮控制相机的缩放
        view_cam.orthographicSize = Mathf.Clamp(view_cam.orthographicSize + Input.mouseScrollDelta.y, 3, 7);

        if(Input.GetMouseButtonDown(1)) last = Input.mousePosition;

        // 当按住鼠标右键时，根据鼠标移动的距离来移动相机位置
        if (Input.GetMouseButton(1))
        {
            Vector3 deltaMousePosition = Input.mousePosition - last;
            deltaMousePosition.Scale(view_cam.cameraToWorldMatrix.lossyScale * (view_cam.orthographicSize * Mathf.Sqrt(s)));

            deltaMousePosition.Scale(new Vector3(1, -1, 1));
            view_cam.transform.position += deltaMousePosition;
        }

        last = Input.mousePosition; // 更新鼠标位置
    }
}
