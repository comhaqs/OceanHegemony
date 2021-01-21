using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiListSkill : MonoBehaviour
{
    public List<UiTouch> touchs;

    List<Item> nodes;
    private void OnEnable()
    {
        MessageManager.GetInstance().Add("ui_person_skill_update", OnUiPersonSkillUpdate, gameObject);
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
                    var skill = nodes[index].GetComponent<ItemSkill>();
                    nodes.RemoveAt(index);
                    UpdateUi();
                    if (null != skill) {
                        skill.OnAction();
                    }
                }
            });
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiPersonSkillUpdate()
    {
        var param = new InfoParam1<Person>();
        MessageManager.GetInstance().Notify("person_player", param);
        if (null == param.param1) {
            return;
        }
        nodes = param.param1.items;
        UpdateUi();
    }

    void UpdateUi() {
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = 0; i < nodes.Count && i < touchs.Count; ++i)
        {
            touchs[i].gameObject.SetActive(true);
            touchs[i].img.sprite = Resources.Load<Sprite>(nodes[i].icon_file);
        }
    }
}
