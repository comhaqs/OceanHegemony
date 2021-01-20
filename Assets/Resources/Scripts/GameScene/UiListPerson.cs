using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class UiListPerson : MonoBehaviour
{
    public List<UiTouch> touchs;

    List<Person> nodes;
    private void OnEnable()
    {
        MessageManager.GetInstance().Add<List<Person>>("ui_person_search", OnUiItemSearch, gameObject);
    }
    public virtual void Start()
    {
        for (int i = 0; i < touchs.Count; ++i)
        {
            var index = i;
            touchs[i].bt.onClick.AddListener(() =>
            {
                if (null != nodes && index < nodes.Count)
                {
                    MessageManager.GetInstance().Notify("ui_person_click", nodes[i]);
                }
            });
            touchs[i].img.sprite = Resources.Load<Sprite>("Sprites/person_0");
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiItemSearch(List<Person> ps)
    {
        nodes = ps;
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = 0; i < nodes.Count && i < touchs.Count; ++i)
        {
            touchs[i].gameObject.SetActive(true);
            touchs[i].nm.text = nodes[i].nm;
        }
    }
}
