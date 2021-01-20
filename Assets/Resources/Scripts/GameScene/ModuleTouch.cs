using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ModuleTouch : MonoBehaviour
{
    Vector3 pos_last;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (! IsPointerOverUIObject(Input.mousePosition)) {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                MessageManager.GetInstance().Notify("ui_touch_press", pos);
            }
        } else if (Input.GetMouseButtonUp(0)) {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                MessageManager.GetInstance().Notify("ui_touch_release", pos);
            }
        } else if (Input.GetMouseButton(0)) {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (0.1f < (pos - pos_last).magnitude)
                {
                    pos_last = pos;
                    MessageManager.GetInstance().Notify("ui_touch_move", pos);
                }
            }
        }
    }
    bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
