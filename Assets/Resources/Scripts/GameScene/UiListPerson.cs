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
        MessageManager.GetInstance().Add<List<Person>>("ui_player_person_update", OnUiIPlayerPersonUpdate, gameObject);
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
                    MessageManager.GetInstance().Notify("ui_person_click", nodes[index]);
                }
            });
            touchs[i].img.sprite = Resources.Load<Sprite>("Sprites/person_0");
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiIPlayerPersonUpdate(List<Person> ps)
    {
        nodes = ps;
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = Mathf.Min(nodes.Count - 1, touchs.Count - 1); i >= 0; --i)
        {
            if (null == nodes[i]) {
                nodes.RemoveAt(i);
                continue;
            }
            touchs[i].gameObject.SetActive(true);
            touchs[i].nm.text = nodes[i].nm;
            touchs[i].bt.interactable = nodes[i].flag_show;
        }
    }
}
