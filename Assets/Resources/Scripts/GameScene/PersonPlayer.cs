using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class PersonPlayer : Person
{
    public ModulePath path_template;
    public List<ModulePath> paths = new List<ModulePath>();
    List<ModulePath> pools = new List<ModulePath>();
    Vector3 pos_last;
    bool flag_move = false;

    public override void Start()
    {
        nm = "自己";
        base.Start();
        MessageManager.GetInstance().Notify("ui_person_skill_update");
        MessageManager.GetInstance().Notify("map_node_update", gameObject);
        MessageManager.GetInstance().Notify("ui_player_info_update", this as Person);
        StartCoroutine(CheckMp());
    }
    void OnEnable()
    {
        MessageManager.GetInstance().Add<GameObject>("person_touch_item", OnPersonTouchItem, gameObject);
        MessageManager.GetInstance().Add<Person>("ui_person_click", OnUiPersonClick, gameObject);
        MessageManager.GetInstance().Add<InfoParam1<Person>>("person_player", OnPersonPlayer, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_press", OnPathPress, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_move", OnPathMove, gameObject);
        MessageManager.GetInstance().Add<Vector3>("ui_touch_release", OnPathRelease, gameObject);
        MessageManager.GetInstance().Add("move_to_postion", OnMoveToPosition, gameObject);
        MessageManager.GetInstance().Add("action_move", OnActionMove, gameObject);
    }

    void OnPersonTouchItem(GameObject obj)
    {
        MessageManager.GetInstance().Notify("ui_item_select", new InfoParam2<GameObject, GameObject>() { param1 = obj, param2 = gameObject});
    }

    void OnUiPersonClick(Person enemy)
    {
        DoAttack(enemy);
    }

    void OnPersonPlayer(InfoParam1<Person> param) {
        param.param1 = this;
    }

    protected override void OnIncreaseMp()
    {
        var old_mp = mp;
        base.OnIncreaseMp();
        if (old_mp != mp) {
            MessageManager.GetInstance().Notify("ui_player_mp_update", 1.0f * mp / mp_max);
        }
    }

    void OnPathPress(Vector3 pos)
    {
        foreach (var p in paths) {
            p.gameObject.SetActive(false);
            pools.Add(p);
        }
        paths.Clear();
        var pos_normal = UtilityTool.NormalSize(pos);
        AddPath(pos_normal);
    }

    void OnPathMove(Vector3 pos)
    {
        var pos_normal = UtilityTool.NormalSize(pos);
        AddPath(pos_normal);
    }

    void OnPathRelease(Vector3 pos)
    {
        //OnMoveToPosition();
    }

    int index_path = 0;
    void AddPath(Vector3 pos)
    {
        if (flag_move)
        {
            return;
        }
        if (0 == paths.Count || pos != pos_last)
        {
            if (0 == paths.Count)
            {
                pos_last = transform.position;
            }
            int x_start = UtilityTool.ToIndexXY(pos_last.x);
            int y_start = UtilityTool.ToIndexXY(pos_last.y);
            int x_end = UtilityTool.ToIndexXY(pos.x);
            int y_end = UtilityTool.ToIndexXY(pos.y);
            int step_x = x_end > x_start ? 1 : -1;
            int step_y = y_end > y_start ? 1 : -1;
            ModulePath obj = null;
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
                CalcAllPath((p_x, p_y)=> {
                    if (0 < pools.Count)
                    {
                        obj = pools[pools.Count - 1];
                        pools.RemoveAt(pools.Count - 1);
                        obj.gameObject.SetActive(true);
                    }
                    else
                    {
                        obj = Instantiate(path_template);
                        obj.name = string.Format("path_{0}", index_path++);
                    }
                    obj.transform.position = UtilityTool.ToPosition(p_x, p_y);
                    paths.Add(obj);
                }, x_last, y_last, x, y);
                x_last = x;
                y_last = y;
            }
            pos_last = pos;

            CheckPath();
        }
    }

    void OnMoveToPosition()
    {
        StartCoroutine(MoveToPosition());
    }
    IEnumerator MoveToPosition()
    {
        if (0 == paths.Count)
        {
            yield break;
        }
        flag_move = true;
        MessageManager.GetInstance().Notify("ui_item_search", new List<Item>());
        MessageManager.GetInstance().Notify("ui_person_search", new List<Person>());
        var paths_tmp = new List<ModulePath>();
        int i = 0;
        for (; i < paths.Count; ++i) {
            if (mp < paths[i].weight)
            {
                break;
            }
            mp -= paths[i].weight;
            paths_tmp.Add(paths[i]);
        }
        paths.RemoveRange(0, i);
        if (0 < paths_tmp.Count)
        {
            MessageManager.GetInstance().Notify("ui_player_mp_update", 1.0f * mp / mp_max);
            foreach (var n in paths_tmp)
            {
                n.gameObject.SetActive(false);
                pools.Add(n);
                var pos = n.transform.position;
                transform.DOMove(pos, 0.5f);
                MessageManager.GetInstance().Notify("map_block_update", pos);
                yield return new WaitForSeconds(0.5f);
            }
            MessageManager.GetInstance().Notify("map_owner_update", this as Person);
        }
        flag_move = false;
    }

    IEnumerator CheckMp() {
        while (true) {
            yield return new WaitForSeconds(1.0f);

            CheckPath();
            CheckListItem();
            CheckListPerson();
        }
    }

    void CheckPath() {
        if (flag_move || 0 == paths.Count ) {
            return;
        }
        var param = new InfoParam1<List<ModulePath>>() { param1 = paths };
        MessageManager.GetInstance().Notify("map_path_info", param);
        int mp_tmp = mp;
        int count_move = 0;
        int i = 0;
        for (; i < paths.Count; ++i) {
            if (mp_tmp >= paths[i].weight)
            {
                paths[i].type = PathType.PATH_ALLOW;
                mp_tmp -= paths[i].weight;
                ++count_move;
            }
            else {
                break;
            }
        }
        for (; i < paths.Count; ++i)
        {
            paths[i].type = PathType.PATH_FORBID;
        }
        MessageManager.GetInstance().Notify("ui_action_move_update", 60 <= mp && 0 < count_move);
    }

    void CheckListItem() {
        var param = new InfoParam3<List<Item>, Vector3, int>() { param2 = transform.position, param3 = 1 };
        MessageManager.GetInstance().Notify("map_item_search", param);
        MessageManager.GetInstance().Notify("ui_item_search", param.param1);
    }

    void CheckListPerson() {
        var param = new InfoParam3<List<Person>, Vector3, int>() { param2 = transform.position, param3 = 2 };
        MessageManager.GetInstance().Notify("map_person_range", param);
        var enemys = new List<Person>();
        foreach (var p in param.param1)
        {
            if (p.camp != camp)
            {
                enemys.Add(p);
            }
        }
        MessageManager.GetInstance().Notify("ui_person_search", enemys);
    }

    void OnActionMove() {
        OnMoveToPosition();
    }

    void CalcAllPath(Action<int, int> action, int x_start, int y_start, int x_end, int y_end) {
        var x_step = x_end - x_start;
        if (0 <= x_step)
        {
            for (int i = x_start + 1; i <= x_end; ++i)
            {
                action(i, y_start);
            }
        }
        else {
            for (int i = x_start - 1; i >= x_end; --i)
            {
                action(i, y_start);
            }
        }
        var y_step = y_end - y_start;
        if (0 <= y_step)
        {
            for (int j = y_start + 1; j <= y_end; ++j)
            {
                action(x_end, j);
            }
        }
        else {
            for (int j = y_start - 1; j >= y_end; --j) {
                action(x_end, j);
            }
        }
    }
}
