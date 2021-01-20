using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ModuleMove : MonoBehaviour
{
    public GameObject path_template;
    List<GameObject> paths = new List<GameObject>();
    List<GameObject> pools = new List<GameObject>();
    Vector3 pos_last;
    bool flag_move = false;
    void Start()
    {
        MessageManager.GetInstance().Add<Vector3>("ui_touch_press", OnPathPress, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_move", OnPathMove, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_release", OnPathRelease, gameObject);
        MessageManager.GetInstance().Add("move_to_postion", OnMoveToPosition, gameObject);
    }

    void OnPathPress(Vector3 pos) {
        var pos_normal = UtilityTool.NormalSize(pos);
        AddPath(pos_normal);
    }

    void OnPathMove(Vector3 pos) {
        var pos_normal = UtilityTool.NormalSize(pos);
        AddPath(pos_normal);
    }

    void OnPathRelease(Vector3 pos) {
        OnMoveToPosition();
    }

    void AddPath(Vector3 pos) {
        if (flag_move) {
            return;
        }
        if (0 == paths.Count || pos != pos_last) {
            if (0 == paths.Count) {
                pos_last = transform.position;
            }
            int x_start = UtilityTool.ToIndexXY(pos_last.x);
            int y_start = UtilityTool.ToIndexXY(pos_last.y);
            int x_end = UtilityTool.ToIndexXY(pos.x);
            int y_end = UtilityTool.ToIndexXY(pos.y);
            int step_x = x_end > x_start ? 1 : -1;
            int step_y = y_end > y_start ? 1 : -1;
            GameObject obj = null;
            int x = 0;
            int y = 0;
            int step_count = Mathf.Max(Mathf.Abs(x_end - x_start), Mathf.Abs(y_end - y_start));
            if (0 == step_count)
            {
                return;
            }
            float t_x = 1.0f * (x_end - x_start) / step_count;
            float t_y = 1.0f * (y_end - y_start) / step_count;
            int x_last = x_start;
            int y_last = y_start;
            for (int i = 1; i <= step_count; ++i)
            {
                x = Mathf.FloorToInt(x_start + t_x * i);
                y = Mathf.FloorToInt(y_start + t_y * i);
                var v = Mathf.Abs(x - x_last) + Mathf.Abs(y - y_last);
                if (2 == v)
                {
                    if (0 < pools.Count)
                    {
                        obj = pools[pools.Count - 1];
                        pools.RemoveAt(pools.Count - 1);
                        obj.SetActive(true);
                    }
                    else
                    {
                        obj = Instantiate(path_template);
                    }
                    obj.transform.position = UtilityTool.ToPosition(x_last, y);
                    paths.Add(obj);

                    if (0 < pools.Count)
                    {
                        obj = pools[pools.Count - 1];
                        pools.RemoveAt(pools.Count - 1);
                        obj.SetActive(true);
                    }
                    else
                    {
                        obj = Instantiate(path_template);
                    }
                    obj.transform.position = UtilityTool.ToPosition(x, y);
                    paths.Add(obj);
                }
                else if (1 == v)
                {
                    if (0 < pools.Count)
                    {
                        obj = pools[pools.Count - 1];
                        pools.RemoveAt(pools.Count - 1);
                        obj.SetActive(true);
                    }
                    else
                    {
                        obj = Instantiate(path_template);
                    }
                    obj.transform.position = UtilityTool.ToPosition(x, y);
                    paths.Add(obj);
                }
                else
                {
                    UtilityTool.LogError("位置错误:" + x_last + "," + y_last + " -> " + x + "," + y + " = " + v);
                }
                x_last = x;
                y_last = y;
            }
            pos_last = pos;
        }
    }

    void OnMoveToPosition()
    {
        StartCoroutine(MoveToPosition());
    }
    IEnumerator MoveToPosition() {
        if (0 == paths.Count) {
            yield break;
        }
        flag_move = true;
        MessageManager.GetInstance().Notify("ui_item_search", new List<Item>());
        MessageManager.GetInstance().Notify("ui_person_search", new List<Person>());
        while (0 < paths.Count) {
            var n = paths[0];
            paths.RemoveAt(0);
            n.SetActive(false);
            pools.Add(n);
            var pos = n.transform.position;
            transform.DOMove(pos, 0.5f);
            MessageManager.GetInstance().Notify("map_person_update", gameObject);
            MessageManager.GetInstance().Notify("map_block_update", pos);
            yield return new WaitForSeconds(0.5f);
            if (0 == paths.Count) {
                //MessageManager.GetInstance().Notify("map_owner_update", gameObject);
                MessageManager.GetInstance().Notify("map_item_search", pos);
                MessageManager.GetInstance().Notify("map_enemy_search", gameObject);
            }
        }
        flag_move = false;
    }
}
