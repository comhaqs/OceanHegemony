using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiListSkill : MonoBehaviour
{
    public List<UiTouch> touchs;

    List<Item> nodes;
    private void OnEnable()
    {
        MessageManager.GetInstance().Add<List<Item>>("ui_person_skill_update", OnUiPersonSkillUpdate, gameObject);
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
                    if (null != skill) {
                        MessageManager.GetInstance().Notify("player_skill_click", skill);
                    }
                }
            });
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiPersonSkillUpdate(List<Item> items)
    {
        nodes = items;
        UpdateUi();
    }

    void UpdateUi() {
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = Mathf.Min(nodes.Count - 1, touchs.Count - 1); i >= 0; --i)
        {
            if (null == nodes[i])
            {
                nodes.RemoveAt(i);
                continue;
            }
            touchs[i].gameObject.SetActive(true);
            touchs[i].img.sprite = Resources.Load<Sprite>(nodes[i].icon_file);
            touchs[i].bt.interactable = nodes[i].flag_show;
        }
    }
}
