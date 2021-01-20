﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InfoBlock {
    public int index = 0;
    public int x = 0;
    public int y = 0;
    public int F = 0;
    public int G = 0;
    public int H = 0;
    public ModuleBlock block;
    public InfoBlock parent;
    public Dictionary<int, GameObject> nodes = new Dictionary<int, GameObject>();
    public GameObject owner;
}

public class ModuleMap : MonoBehaviour
{
    public int size_x = 101;
    public int size_y = 101;
    public ModuleBlock block_template;
    public GameObject root;
    public MeshFilter mesh_filter;
    public ItemTreasure treasure_template;
    public PersonEnemy enemy_template;

    Dictionary<int, InfoBlock> blocks = new Dictionary<int, InfoBlock>();
    Dictionary<int, InfoBlock> nodes = new Dictionary<int, InfoBlock>();
    static int index_enemy = 0;
    void Start()
    {
        if (1 != (size_x % 2)) {
            size_x += 1;
        }
        if (1 != (size_y % 2)) {
            size_y += 1;
        }
        MessageManager.GetInstance().Add<InfoParam3<List<Vector3>, Vector3, Vector3>>("map_find_path", OnMapFindPath, gameObject);
        MessageManager.GetInstance().Add<Vector3>("map_block_update", OnMapBlockUpdate, gameObject);
        MessageManager.GetInstance().Add<GameObject>("map_person_update", OnMapPersonUpdate, gameObject);
        MessageManager.GetInstance().Add<GameObject>("map_owner_update", OnMapOwnerUpdate, gameObject);
        MessageManager.GetInstance().Add<Vector3>("map_item_search", OnMapItemSearch, gameObject);
        MessageManager.GetInstance().Add<GameObject>("map_enemy_search", OnMapEnemySearch, gameObject);
        MessageManager.GetInstance().Add<InfoParam1<Vector3>>("map_position_random", OnMapPositionRandom, gameObject);
        MessageManager.GetInstance().Add<InfoParam3<List<Person>, Vector3, int>>("map_person_range", OnMapPersonRange, gameObject);
        StartCoroutine(InitMap());
        
    }

    IEnumerator InitMap()
    {
        yield return null;
        if (null == root) {
            root = gameObject;
        }
        for (int i = -Mathf.FloorToInt(size_x / 2); i <= Mathf.FloorToInt(size_x / 2); ++i) {
            for (int j = -Mathf.FloorToInt(size_y / 2); j <= Mathf.FloorToInt(size_y / 2); ++j) {
                var info = new InfoBlock() { index = ToIndex(i, j), x = i, y = j, block = Instantiate(block_template, root.transform) };
                info.block.name = string.Format("block_{0}_{1}", i, j);
                info.block.transform.localPosition = UtilityTool.ToPosition(info.x, info.y);
                info.block.type = BLOCK_TYPE.BLOCK_OCEAN;
                blocks[info.index] = info;
            }
        }
        for (int m = 0; m < 10; ++m) {
            int s = UnityEngine.Random.Range(6, 12);
            var x = UnityEngine.Random.Range(-Mathf.FloorToInt(size_x / 2), Mathf.FloorToInt(size_x / 2) - s);
            var y = UnityEngine.Random.Range(-Mathf.FloorToInt(size_y / 2), Mathf.FloorToInt(size_y / 2) - s);
            for (int i = 0; i < s; ++i)
            {
                for (int j = 0; j < s; ++j)
                {
                    blocks[ToIndex(x + i, y + j)].block.type = BLOCK_TYPE.BLOCK_LAND;
                }
            }
        }
        StartCoroutine(ScaleMap());
        StartCoroutine(DoTreasure());
        StartCoroutine(DoEnemy());
    }

    int ToIndex(int x, int y) {
        return x * size_y + y;
    }
    

    int ToIndex(Vector3 pos) {
        int x = UtilityTool.ToIndexXY(pos.x);
        int y = UtilityTool.ToIndexXY(pos.y);
        return ToIndex(x, y);
    }

    void GetRanges(ref List<InfoBlock> ranges, InfoBlock info) {
        ranges.Clear();
        if (0 < info.x)
        {
            ranges.Add(blocks[ToIndex(info.x - 1, info.y)]);
        };
        if (size_x - 1 > info.x) {
            ranges.Add(blocks[ToIndex(info.x + 1, info.y)]);
        }
        if (0 < info.y)
        {
            ranges.Add(blocks[ToIndex(info.x, info.y - 1)]);
        };
        if (size_y - 1 > info.y)
        {
            ranges.Add(blocks[ToIndex(info.x, info.y + 1)]);
        }
    }

    void OnCalcPath(ref List<Vector3> paths, Vector3 pos_start, Vector3 pos_end) {
        paths.Clear();
        var index_start = ToIndex(pos_start);
        var info_start = blocks[index_start];
        var index_end = ToIndex(pos_end);
        var info_end = blocks[index_end];
        if (0 > index_start || 0 > index_end) {
            return;
        }
        if (index_start == index_end) {
            paths.Add(pos_end);
            return;
        }

        var list_open = new Dictionary<int, InfoBlock>();
        var list_close = new Dictionary<int, InfoBlock>();
        var ranges = new List<InfoBlock>();
        InfoBlock info_current = new InfoBlock() { index = info_start.index, x = info_start.x, y = info_start.y, block = info_start.block};
        list_open.Add(info_current.index, info_current);
        while (0 < list_open.Count) {
            info_current = null;
            foreach (var kv in list_open) {
                if (null == info_current || kv.Value.F < info_current.F)
                {
                    info_current = kv.Value;
                }
            }
            list_open.Remove(info_current.index);
            list_close.Add(info_current.index, info_current);
            if (info_current.index == index_end) {
                while (null != info_current && index_start != info_current.index) {
                    paths.Add(info_current.block.transform.position);
                    info_current = info_current.parent;
                }
                paths.Reverse();
                return;
            }

            ranges.Clear();
            GetRanges(ref ranges, info_current);
            foreach (var d in ranges) {
                if (list_close.ContainsKey(d.index))
                {
                }
                else {
                    var info_check = new InfoBlock() { index = d.index, x = d.x, y = d.y, block = d.block, parent = info_current };
                    info_check.G = info_current.G + info_check.block.weigtht;
                    info_check.H = Mathf.Abs(info_end.x - d.x) + Mathf.Abs(info_end.y - d.y);
                    info_check.F = info_check.G + info_check.H;
                    var info_open = list_open[info_check.index];
                    if (null == info_open)
                    {
                        list_open.Add(info_check.index, info_check);
                    }
                    else if(info_open.F > info_check.F)
                    {
                        info_open.G = info_check.G;
                        info_open.H = info_check.H;
                        info_open.F = info_check.F;
                        info_open.parent = info_check.parent;
                    }
                }
            }
        }
    }

    void OnCalcPathWithPower(ref List<Vector3> paths, Vector3 pos_start, int power)
    {
        paths.Clear();
        var index_start = ToIndex(pos_start);
        var info_start = blocks[index_start];

        var list_open = new Dictionary<int, InfoBlock>();
        var list_close = new Dictionary<int, InfoBlock>();
        var ranges = new List<InfoBlock>();
        InfoBlock info_current = new InfoBlock() { index = info_start.index, x = info_start.x, y = info_start.y, block = info_start.block };
        list_open.Add(info_current.index, info_current);
        while (0 < list_open.Count)
        {
            info_current = null;
            foreach (var kv in list_open)
            {
                if (null == info_current || kv.Value.F < info_current.F)
                {
                    info_current = kv.Value;
                }
            }
            list_open.Remove(info_current.index);
            list_close.Add(info_current.index, info_current);

            ranges.Clear();
            GetRanges(ref ranges, info_current);
            foreach (var d in ranges)
            {
                if (list_close.ContainsKey(d.index))
                {
                }
                else
                {
                    var info_check = new InfoBlock() { index = d.index, x = d.x, y = d.y, block = d.block, parent = info_current };
                    info_check.G = info_current.G + info_check.block.weigtht;
                    info_check.H = 0;
                    info_check.F = info_check.G + info_check.H;
                    var info_open = list_open[info_check.index];
                    if (null == info_open)
                    {
                        if (power < info_check.F)
                        {
                            continue;
                        }
                        list_open.Add(info_check.index, info_check);
                    }
                    else if (info_open.F > info_check.F)
                    {
                        info_open.G = info_check.G;
                        info_open.H = info_check.H;
                        info_open.F = info_check.F;
                        info_open.parent = info_check.parent;
                    }
                }
            }
        }
        foreach (var kv in list_close) {
            if (power >= kv.Value.F) {
                paths.Add(kv.Value.block.transform.position);
            }
        }
    }

    void OnMapFindPath(InfoParam3<List<Vector3>, Vector3, Vector3> param) {
        OnCalcPath(ref param.param1, param.param2, param.param3);
    }

    void OnMapBlockUpdate(Vector3 pos) {
        var info = blocks[ToIndex(pos)];
        info.block.type = BLOCK_TYPE.BLOCK_LAND;
    }

    void OnMapPersonUpdate(GameObject obj) {
        UpdateMapNode(obj);
    }

    void OnMapOwnerUpdate(GameObject obj) {
        var info = blocks[ToIndex(obj.transform.position)];
        for (int i = info.x - 1; i <= info.x + 1; ++i) {
            for (int j = info.y - 1; j <= info.y + 1; ++i) {
                if (0 <= i && i < size_x && 0 <=j && j < size_y) {
                    var info_check = blocks[ToIndex(i, j)];
                    if (BLOCK_TYPE.BLOCK_OCEAN != info_check.block.type) {
                        info_check.block.type = BLOCK_TYPE.BLOCK_SELF;
                        info_check.owner = obj;
                    }
                }
            }
        }
    }

    void OnMapItemSearch(Vector3 pos)
    {
        var items = new List<Item>();
        var info = blocks[ToIndex(pos)];
        for (int i = info.x - 1; i <= info.x + 1; ++i)
        {
            for (int j = info.y - 1; j <= info.y + 1; ++j)
            {
                if (Mathf.FloorToInt(-size_x/2) <= i && i <= Mathf.FloorToInt(size_x / 2) 
                    && Mathf.FloorToInt(-size_y / 2) <= j && j <= Mathf.FloorToInt(size_y / 2))
                {
                    var info_check = blocks[ToIndex(i, j)];
                    foreach (var m in info_check.nodes.Keys) {
                        if (null == info_check.nodes[m]) {
                            info_check.nodes.Remove(m);
                            nodes.Remove(m);
                            continue;
                        }
                        var item = info_check.nodes[m].GetComponent<Item>();
                        if (null != item)
                        {
                            items.Add(item);
                        }
                    }
                }
            }
        }
        MessageManager.GetInstance().Notify("ui_item_search", items);
    }

    void OnMapEnemySearch(GameObject obj)
    {
        var persons = new List<Person>();
        var info = blocks[ToIndex(obj.transform.position)];
        for (int i = info.x - 1; i <= info.x + 1; ++i)
        {
            for (int j = info.y - 1; j <= info.y + 1; ++j)
            {
                if (Mathf.FloorToInt(-size_x / 2) <= i && i <= Mathf.FloorToInt(size_x / 2)
                    && Mathf.FloorToInt(-size_y / 2) <= j && j <= Mathf.FloorToInt(size_y / 2))
                {
                    var info_check = blocks[ToIndex(i, j)];
                    foreach (var m in info_check.nodes.Keys)
                    {
                        if (null == info_check.nodes[m])
                        {
                            info_check.nodes.Remove(m);
                            nodes.Remove(m);
                            continue;
                        }
                        var person = info_check.nodes[m].GetComponent<Person>();
                        if (null != person && obj != person.gameObject)
                        {
                            persons.Add(person);
                        }
                    }
                }
            }
        }
        MessageManager.GetInstance().Notify("ui_person_search", persons);
    }

    IEnumerator ScaleMap() {
        yield return new WaitForSeconds(1.0f);
        int step_x = Mathf.FloorToInt(size_x * 0.15f);
        int step_y = Mathf.FloorToInt(size_y * 0.15f);
        var block_left_bottton = blocks[ToIndex(-Mathf.FloorToInt(size_x / 2), -Mathf.FloorToInt(size_y / 2))];
        var block_top_right = blocks[ToIndex(Mathf.FloorToInt(size_x / 2), Mathf.FloorToInt(size_y / 2))];
        RectInt rect_origin = new RectInt(block_left_bottton.x, block_left_bottton.y, block_top_right.x - block_left_bottton.x, block_top_right.y - block_left_bottton.y);
        RectInt rect_old = new RectInt(rect_origin.x, rect_origin.y, rect_origin.width, rect_origin.height);
        RectInt rect_new = new RectInt(rect_old.x, rect_old.y, rect_old.width, rect_old.height);
        while (true) {
            yield return new WaitForSeconds(10.0f);
            if (!mesh_filter.gameObject.activeSelf) {
                mesh_filter.gameObject.SetActive(true);
            }
            rect_new.x = rect_old.x + UnityEngine.Random.Range(0, step_x);
            rect_new.y = rect_old.y + UnityEngine.Random.Range(0, step_y);
            rect_new.width = rect_old.width - step_x;
            rect_new.height = rect_old.height - step_y;
            if (rect_new.x > rect_old.x + rect_old.width) {
                rect_new.x = rect_old.x;
            }
            if (rect_new.y > rect_old.y + rect_old.height)
            {
                rect_new.y = rect_old.y;
            }
            if (4 > rect_new.width || 4 > rect_new.height)
            {
                rect_new.width = 0;
                rect_new.height = 0;
            }

            int count_step = 12;
            for (int i = 1; i <= count_step; ++i) {
                var points = new List<Vector3>();
                int x = rect_old.x + Mathf.FloorToInt((rect_new.x - rect_old.x) * i / count_step);
                int y = rect_old.y + Mathf.FloorToInt((rect_new.y - rect_old.y) * i / count_step);
                int width = rect_old.width + Mathf.FloorToInt((rect_new.width - rect_old.width) * i / count_step);
                int height = rect_old.height + Mathf.FloorToInt((rect_new.height - rect_old.height) * i / count_step);
                points.Add(UtilityTool.ToPosition(rect_origin.x, rect_origin.y) + 0.5f * new Vector3(-UtilityTool.block_size, -UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(rect_origin.x, rect_origin.y + rect_origin.height) + 0.5f * new Vector3(-UtilityTool.block_size, UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(rect_origin.x + rect_origin.width, rect_origin.y + rect_origin.height) + 0.5f * new Vector3(UtilityTool.block_size, UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(rect_origin.x + rect_origin.width, rect_origin.y) + 0.5f * new Vector3(UtilityTool.block_size, -UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(x, y) + 0.5f * new Vector3(-UtilityTool.block_size, -UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(x, y + height) + 0.5f * new Vector3(-UtilityTool.block_size, UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(x + width, y +height) + 0.5f * new Vector3(UtilityTool.block_size, UtilityTool.block_size, 0.0f));
                points.Add(UtilityTool.ToPosition(x + width, y) + 0.5f * new Vector3(UtilityTool.block_size, -UtilityTool.block_size, 0.0f));
                Mesh mesh = new Mesh();
                mesh.vertices = points.ToArray();
                var triangles = new List<int>() { 0,1,5, 5,4,0, 1,2,6, 6,5,1, 2,3,7, 7,6,2, 3,0,4, 4,7,3};
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                mesh_filter.mesh = mesh;

                int time_action = Mathf.FloorToInt(12.0f / count_step);
                for (int j = 0; j < time_action; ++j) {
                    var list_blocks = new Dictionary<int, InfoBlock>();
                    foreach (var kv in nodes) {
                        if (list_blocks.ContainsKey(kv.Value.index)) {
                            continue;
                        }
                        if (kv.Value.x <= rect_new.x || kv.Value.x >= rect_new.x + rect_new.width
                            || kv.Value.y <= rect_new.y || kv.Value.y >= rect_new.y + rect_new.height) {
                            list_blocks.Add(kv.Value.index, kv.Value);
                        }
                    }
                    foreach (var kv in list_blocks) {
                        foreach (var k in kv.Value.nodes.Keys) {
                            if (null == kv.Value.nodes[k]) {
                                kv.Value.nodes.Remove(k);
                                nodes.Remove(k);
                                continue;
                            }
                            var person = kv.Value.nodes[k].GetComponent<Person>();
                            if (null != person) {
                                person.hp -= 2;
                            }
                        }
                    }
                    yield return new WaitForSeconds(time_action);
                }
                
            }
            if (0 == rect_new.width || 0 >= rect_new.height) {
                yield break;
            }
            rect_old = rect_new;
        }
    }

    IEnumerator DoTreasure() {
        for (int i = 0; i < 100; ++i) {
            AddTreasure();
        }
        while (true) {
            yield return new WaitForSeconds(5.0f);
            AddTreasure();
        }
    }

    void AddTreasure() {
        var treasure = Instantiate(treasure_template, transform);
        treasure.transform.position = RandomPosition();
        var index = ToIndex(treasure.transform.position);
        var info = blocks[index];
        info.nodes.Add(treasure.GetInstanceID(), treasure.gameObject);
        nodes[treasure.GetInstanceID()] = info;
    }

    Vector3 RandomPosition() {
        return UtilityTool.ToPosition(UnityEngine.Random.Range(-Mathf.FloorToInt(size_x / 2), Mathf.FloorToInt(size_x / 2))
            , UnityEngine.Random.Range(-Mathf.FloorToInt(size_y / 2), Mathf.FloorToInt(size_y / 2)));
    }

    void OnMapPositionRandom(InfoParam1<Vector3> param) {
        param.param1 = RandomPosition();
    }

    IEnumerator DoEnemy() {
        for (int i = 0; i < 15; ++i) {
            AddEnemy();
        }
        while (true) {
            yield return new WaitForSeconds(10.0f);
            AddEnemy();
        }
    }

    void AddEnemy() {
        var enemy = Instantiate(enemy_template, transform);
        enemy.transform.position = RandomPosition();
        enemy.attack = UnityEngine.Random.Range(10, 20);
        enemy.nm = string.Format("eny_{0}", index_enemy++);
        var index = ToIndex(enemy.transform.position);
        var info = blocks[index];
        info.nodes.Add(enemy.GetInstanceID(), enemy.gameObject);
        nodes[enemy.GetInstanceID()] = info;
    }

    void OnMapPersonRange(InfoParam3<List<Person>, Vector3, int> param) {
        param.param1 = new List<Person>();
        int x = UtilityTool.ToIndexXY(param.param2.x);
        int y = UtilityTool.ToIndexXY(param.param2.y);
        int step = Mathf.FloorToInt(param.param3 / 2);
        for (int i = x - step; i <= x + step; ++i) {
            for (int j = y - step; j <= y + step; ++j) {
                if (Mathf.FloorToInt(-size_x / 2) <= i && Mathf.FloorToInt(size_x / 2) >= i
                    && Mathf.FloorToInt(-size_y / 2) <= j && Mathf.FloorToInt(size_y / 2) >= j) {
                    var info = blocks[ToIndex(i, j)];
                    foreach (var k in info.nodes.Keys) {
                        if (null == info.nodes[k]) {
                            info.nodes.Remove(k);
                            nodes.Remove(k);
                            continue;
                        }
                        var person = info.nodes[k].GetComponent<Person>();
                        if (null != person) {
                            param.param1.Add(person);
                        }
                    }
                }
            }
        }
    }

    void UpdateMapNode(GameObject obj)
    {
        InfoBlock info = null;
        if (nodes.TryGetValue(obj.GetInstanceID(), out info)) {
            foreach (var k in info.nodes.Keys)
            {
                if (null == info.nodes[k])
                {
                    info.nodes.Remove(k);
                    nodes.Remove(k);
                    continue;
                }
                if (obj == info.nodes[k])
                {
                    info.nodes.Remove(k);
                    nodes.Remove(k);
                    break;
                }
            }
        }

        info = blocks[ToIndex(obj.transform.position)];
        info.nodes.Add(obj.GetInstanceID(), obj);
        nodes[obj.GetInstanceID()] = info;
    }
}
