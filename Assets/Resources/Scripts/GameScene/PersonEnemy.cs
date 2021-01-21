using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PersonEnemy : Person
{
    Person target;
    public override void Start()
    {
        StartCoroutine(DoAI());
    }

    IEnumerator DoAI() {
        float probability = 0.0f;
        while (true) {
            yield return new WaitForSeconds(8.0f);
            probability = UnityEngine.Random.Range(0.0f, 1.0f);
            if (null != target && 2.0f * UtilityTool.block_size < (target.transform.position - transform.position).magnitude) {
                target = null;
            }
            if (null != target && 0.8f > probability) {
                DoAttack(target);
                continue;
            }
            if (0.20f > probability)
            {
                // 获取周围敌人
                var param = new InfoParam3<List<Person>, Vector3, int>() { param2 = transform.position, param3 = 8 };
                MessageManager.GetInstance().Notify("map_person_range", param);
                var persons = new List<Person>();
                foreach (var p in param.param1)
                {
                    if (p.camp != camp)
                    {
                        persons.Add(p);
                    }
                }
                if (0 < persons.Count)
                {
                    var person = persons[UnityEngine.Random.Range(0, persons.Count - 1)];
                    target = person;
                    var paths = CalcPath(person.transform.position);
                    if (0 < paths.Count)
                    {
                        StartCoroutine(MoveToPosition(paths));
                    }
                }
            }
            else if (0.25f + 0.2f > probability)
            {
                var param = new InfoParam1<Vector3>();
                MessageManager.GetInstance().Notify("map_position_random", param);
                var paths = CalcPath(param.param1);
                if (6 < paths.Count)
                {
                    paths.RemoveRange(6, paths.Count - 6);
                }
                if (0 < paths.Count)
                {
                    StartCoroutine(MoveToPosition(paths));
                }
            }
            else
            {
            }
        }
    }

    IEnumerator MoveToPosition(List<Vector3> paths) {
        for (int i = 0; i < paths.Count; ++i) {
            transform.DOMove(paths[i], 0.5f);
            yield return new WaitForSeconds(0.5f);
            MessageManager.GetInstance().Notify("map_block_update", transform.position);
        }
    }

    List<Vector3> CalcPath(Vector3 pos)
    {
        var paths = new List<Vector3>();
        int x_start = UtilityTool.ToIndexXY(transform.position.x);
        int y_start = UtilityTool.ToIndexXY(transform.position.y);
        int x_end = UtilityTool.ToIndexXY(pos.x);
        int y_end = UtilityTool.ToIndexXY(pos.y);
        int step_x = x_end > x_start ? 1 : -1;
        int step_y = y_end > y_start ? 1 : -1;
        int x = 0;
        int y = 0;
        int step_count = Mathf.Max(Mathf.Abs(x_end - x_start), Mathf.Abs(y_end - y_start));
        if (0 == step_count) {
            return paths;
        }
        float t_x = 1.0f * (x_end - x_start) / step_count;
        float t_y = 1.0f * (y_end - y_start) / step_count;
        int x_last = x_start;
        int y_last = y_start;
        for (int i = 1; i <= step_count; ++i) {
            x = Mathf.FloorToInt(x_start + t_x * i);
            y = Mathf.FloorToInt(y_start + t_y * i);
            var v = Mathf.Abs(x - x_last) + Mathf.Abs(y - y_last);
            if (2 == v)
            {
                paths.Add(UtilityTool.ToPosition(x_last, y));
                if (x != x_end && y != y_end) {
                    paths.Add(UtilityTool.ToPosition(x, y));
                }
            }
            else if (1 == v)
            {
                if (x != x_end && y != y_end)
                {
                    paths.Add(UtilityTool.ToPosition(x, y));
                }
            }
            else {
                UtilityTool.LogError("位置错误:" + x_last + "," + y_last + " -> " + x + "," + y + " = " + v);
            }
            x_last = x;
            y_last = y;
        }
        return paths;
    }
}
