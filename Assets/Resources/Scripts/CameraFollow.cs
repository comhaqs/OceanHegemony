using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    Transform main_camera;
    public Vector3 offset = Vector3.zero;

    void Start()
    {
        //设置相对偏移
        main_camera = Camera.main.gameObject.transform;
    }

    void Update()
    {
        var pos = transform.position + offset;
        main_camera.position = pos;
    }
}
