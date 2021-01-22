using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiListItem : MonoBehaviour
{
    public List<UiTouch> touchs;

    List<Item> nodes;
    private void OnEnable()
    {
        MessageManager.GetInstance().Add<List<Item>>("ui_player_item_update", OnUiIPlayerItemUpdate, gameObject);
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
                    MessageManager.GetInstance().Notify("player_item_click", nodes[index]);
                }
            });
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiIPlayerItemUpdate(List<Item> ps)
    {
        nodes = ps;
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = Mathf.Min(nodes.Count - 1, touchs.Count - 1); i >= 0; --i) {
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
