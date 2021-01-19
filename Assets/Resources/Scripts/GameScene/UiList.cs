using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UiList<T> : MonoBehaviour
{
    public string msg_update;
    public List<Button> buttons;
    public string msg_click;

    List<T> nodes;
    public virtual void Start()
    {
        for (int i = 0; i < nodes.Count; ++i) {
            var index = i;
            buttons[i].onClick.AddListener(()=>{
                if (null != nodes && index < nodes.Count) {
                    MessageManager.GetInstance().Notify(msg_click, nodes[i]);
                }
            });
        }
        MessageManager.GetInstance().Add<List<T>>(msg_update, OnNodeUpdate, gameObject);
    }

    void OnNodeUpdate(List<T> ps)
    {
        nodes = ps;
        foreach (var b in buttons) {
            b.gameObject.SetActive(false);
        }
        for (int i = 0; i < ps.Count && i < buttons.Count; ++i) {
            buttons[i].gameObject.SetActive(true);
            SetNode(ps[i], buttons[i]);
        }
        
    }
    public virtual void SetNode(T d, Button b) {
    }
}
