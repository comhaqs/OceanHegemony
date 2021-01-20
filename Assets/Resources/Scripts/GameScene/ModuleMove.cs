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
            int x_start = Mathf.FloorToInt(pos_last.x / UtilityTool.block_size);
            int y_start = Mathf.FloorToInt(pos_last.y / UtilityTool.block_size);
            int x_end = Mathf.FloorToInt(pos.x / UtilityTool.block_size);
            int y_end = Mathf.FloorToInt(pos.y / UtilityTool.block_size);
            int step_x = x_end > x_start ? 1 : -1;
            int step_y = y_end > y_start ? 1 : -1;
            GameObject obj = null;
            int x = 0;
            int y = 0;
            for (x = x_start + step_x, y = y_start; (x_end > x_start && x <= x_end) || (x_end <= x_start &&  x >= x_end); x += step_x) {
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
            for (x = x_end, y = y_start + step_y; (y_end > y_start && y <= y_end) || (y_end <= y_start && y >= y_end); y += step_y)
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
            pos_last = pos;
        }
    }

    void OnMoveToPosition()
    {
        StartCoroutine(MoveToPosition());
    }
    IEnumerator MoveToPosition() {
        flag_move = true;
        while (0 < paths.Count) {
            var n = paths[0];
            paths.RemoveAt(0);
            n.SetActive(false);
            pools.Add(n);
            var pos = n.transform.position;
            transform.DOMove(pos, 0.5f);
            yield return new WaitForSeconds(0.5f);
            //MessageManager.GetInstance().Notify("map_person_update", pos);
            //MessageManager.GetInstance().Notify("map_block_update", pos);

            if (0 == paths.Count) {
                //MessageManager.GetInstance().Notify("map_owner_update", pos);
                MessageManager.GetInstance().Notify("map_item_search", pos);
                //MessageManager.GetInstance().Notify("map_enemy_search", pos);
            }
        }
        flag_move = false;
    }
}
