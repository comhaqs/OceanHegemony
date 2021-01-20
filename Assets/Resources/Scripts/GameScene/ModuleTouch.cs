using UnityEngine;
using System.Collections;

public class ModuleTouch : MonoBehaviour
{
    Vector3 pos_last;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MessageManager.GetInstance().Notify("ui_touch_press", pos);
        } else if (Input.GetMouseButtonUp(0)) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MessageManager.GetInstance().Notify("ui_touch_release", pos);
        } else if (Input.GetMouseButton(0)) {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (0.1f < (pos - pos_last).magnitude) {
                pos_last = pos;
                MessageManager.GetInstance().Notify("ui_touch_move", pos);
            }
        }
    }
}
