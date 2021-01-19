using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ModuleMove : MonoBehaviour
{
    int path_type = 0;
    List<Vector3> paths = new List<Vector3>();
    void Start()
    {
        MessageManager.GetInstance().Add<Vector3>("ui_touch_press", OnPathPress, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_move", OnPathMove, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_release", OnPathRelease, gameObject);
        MessageManager.GetInstance().Add<List<Vector3>>("move_to_postion", OnMoveToPosition, gameObject);
    }

    void OnPathPress(Vector3 pos) {
        var pos_normal = UtilityTool.NormalSize(pos);
        if (pos_normal == UtilityTool.NormalSize(transform.position))
        {
            path_type = 1;
        }
        else {
            path_type = 2;
        }
    }

    void OnPathMove(Vector3 pos) {
        var pos_normal = UtilityTool.NormalSize(pos);
        if (1 == path_type) {
            if (0 == paths.Count || paths[paths.Count - 1] != pos_normal)
            {
                paths.Add(pos_normal);
                MessageManager.GetInstance().Notify("person_path", paths);
            }
        } else if (2 == path_type) {
            if (pos_normal != UtilityTool.NormalSize(transform.position)) {
                paths.Clear();
                MessageManager.GetInstance().Notify("map_find_path", new InfoParam3<List<Vector3>, Vector3, Vector3>() {
                    param1 = paths, param2 = transform.position, param3 = pos_normal});
                if (0 < paths.Count) {
                    MessageManager.GetInstance().Notify("person_path", paths);
                }
            }
        }
    }

    void OnPathRelease(Vector3 pos) {
        path_type = 0;
    }

    void OnMoveToPosition(List<Vector3> paths)
    {
        StartCoroutine(MoveToPosition(paths));
    }
    IEnumerator MoveToPosition(List<Vector3> ps) {
        foreach (var p in ps)
        {
            transform.DOMove(p, 0.5f);
            yield return new WaitForSeconds(0.5f);
            MessageManager.GetInstance().Notify("map_person_update", p);
            MessageManager.GetInstance().Notify("map_block_update", p);
        }
        if (0 < ps.Count) {
            var obj = ps[ps.Count - 1];
            MessageManager.GetInstance().Notify("map_owner_update", obj);
            MessageManager.GetInstance().Notify("map_item_search", obj);
            MessageManager.GetInstance().Notify("map_enemy_search", obj);
        }
    }
}
