using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class UiListPerson : MonoBehaviour
{
    public string msg_update;
    public List<UiTouch> touchs;
    public string msg_click;

    List<Person> nodes;
    public virtual void Start()
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            var index = i;
            touchs[i].bt.onClick.AddListener(() =>
            {
                if (null != nodes && index < nodes.Count)
                {
                    MessageManager.GetInstance().Notify(msg_click, nodes[i]);
                }
            });
        }
        MessageManager.GetInstance().Add<List<Person>>(msg_update, OnNodeUpdate, gameObject);
    }

    void OnNodeUpdate(List<Person> ps)
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
    public virtual void SetNode(Person d, UiTouch b)
    {
        b.img.sprite = d.GetComponent<SpriteRenderer>().sprite;
        b.nm.text = d.nm;
    }
}
