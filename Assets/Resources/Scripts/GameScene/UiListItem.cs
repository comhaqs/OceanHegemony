using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiListItem : MonoBehaviour
{
    public string msg_update;
    public List<GameObject> touchs;
    public string msg_click;

    List<Item> nodes;
    public virtual void Start()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            var index = i;
            touchs[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                if (null != nodes && index < nodes.Count)
                {
                    MessageManager.GetInstance().Notify(msg_click, nodes[i]);
                }
            });
        }
        MessageManager.GetInstance().Add<List<Item>>(msg_update, OnNodeUpdate, gameObject);
    }

    void OnNodeUpdate(List<Item> ps)
    {
        nodes = ps;
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = 0; i < ps.Count && i < touchs.Count; ++i)
        {
            touchs[i].gameObject.SetActive(true);
            SetNode(ps[i], touchs[i]);
        }

    }
    public virtual void SetNode(Item d, GameObject b)
    {
        b.GetComponent<Image>().sprite = d.GetComponent<SpriteRenderer>().sprite;
    }
}
