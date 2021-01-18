using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


class InfoMsg<T> {
    public T action;
    public GameObject target;
}

public class InfoParam1<T1>
{
    public T1 param1;
}

public class InfoParam2<T1, T2>
{
    public T1 param1;
    public T2 param2;
}

public class InfoParam3<T1, T2, T3> {
    public T1 param1;
    public T2 param2;
    public T3 param3;
}

public class MessageManager : MonoBehaviour
{
    static MessageManager s_instance = null;
    public static MessageManager GetInstance() {
        if (null == s_instance) {
            var obj = new GameObject("MessageManager");
            s_instance = obj.AddComponent<MessageManager>();
            DontDestroyOnLoad(obj);
        }
        return s_instance;
    }

    Dictionary<string, object> maps = new Dictionary<string, object>();

    void OnDestroy()
    {
        maps.Clear();
    }

    public virtual bool Add(string name, Action action, GameObject target)
    {
        object obj;
        List<InfoMsg<Action>> list;
        if (!maps.TryGetValue(name, out obj))
        {
            list = new List<InfoMsg<Action>>();
            maps.Add(name, list);
        }
        else
        {
            list = obj as List<InfoMsg<Action>>;
            if (null == list)
            {
                UtilityTool.LogError("类型转换失败:" + typeof(InfoMsg<Action>));
                return false;
            }
        }
        list.Add(new InfoMsg<Action> { action = action, target = target });
        return true;
    }

    public virtual bool Add<T>(string name, Action<T> action, GameObject target)
    {
        object obj;
        List<InfoMsg<Action<T>>> list;
        if (!maps.TryGetValue(name, out obj))
        {
            list = new List<InfoMsg<Action<T>>>();
            maps.Add(name, list);
        }
        else
        {
            list = obj as List<InfoMsg<Action<T>>>;
            if (null == list)
            {
                UtilityTool.LogError("类型转换失败:" + typeof(InfoMsg<Action<T>>));
                return false;
            }
        }
        list.Add(new InfoMsg<Action<T>> { action = action, target = target });
        return true;
    }

    public virtual bool Notify(string name) {
        object obj;
        List<InfoMsg<Action>> list;
        if (!maps.TryGetValue(name, out obj))
        {
            UtilityTool.LogError("获取回调失败，标签:" + name);
            return false;
        }
        list = obj as List<InfoMsg<Action>>;
        if (null == list)
        {
            UtilityTool.LogError("类型转换失败:" + typeof(InfoMsg<Action>));
            return false;
        }
        for (int i = 0; i < list.Count; ++i) {
            if (null == list[i].target) {
                list.RemoveAt(i);
                --i;
                continue;
            }
            list[i].action();
        }
        return true;
    }

    public virtual bool Notify<T>(string name, T param1)
    {
        object obj;
        List<InfoMsg<Action<T>>> list;
        if (!maps.TryGetValue(name, out obj))
        {
            UtilityTool.LogError("获取回调失败，标签:" + name);
            return false;
        }
        list = obj as List<InfoMsg<Action<T>>>;
        if (null == list)
        {
            UtilityTool.LogError("类型转换失败:" + typeof(InfoMsg<Action<T>>));
            return false;
        }
        for (int i = 0; i < list.Count; ++i)
        {
            if (null == list[i].target)
            {
                list.RemoveAt(i);
                --i;
                continue;
            }
            list[i].action(param1);
        }
        return true;
    }

    public virtual bool Remove(string name, Action action)
    {
        object obj;
        List<Action> list;
        if (!maps.TryGetValue(name, out obj))
        {
            UtilityTool.LogError("找不到对应的消息:" + name);
            return true;
        }
        list = obj as List<Action>;
        if (null == list)
        {
            UtilityTool.LogError("类型转换失败:" + typeof(Action));
            return false;
        }
        list.Remove(action);
        return true;
    }

    public virtual bool Remove<T>(string name, Action<T> action)
    {
        object obj;
        List<Action<T>> list;
        if (!maps.TryGetValue(name, out obj))
        {
            UtilityTool.LogError("找不到对应的消息:" + name);
            return true;
        }
        list = obj as List<Action<T>>;
        if (null == list)
        {
            UtilityTool.LogError("类型转换失败:" + typeof(List<Action<T>>));
            return false;
        }
        list.Remove(action);
        return true;
    }
}
