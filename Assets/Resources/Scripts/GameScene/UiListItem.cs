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
        MessageManager.GetInstance().Add<List<Item>>("ui_item_search", OnUiItemSearch, gameObject);
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
                    MessageManager.GetInstance().Notify("ui_item_click", nodes[index]);
                }
            });
            touchs[i].gameObject.SetActive(false);
        }
    }

    void OnUiItemSearch(List<Item> ps)
    {
        nodes = ps;
        foreach (var b in touchs)
        {
            b.gameObject.SetActive(false);
        }
        for (int i = 0; i < nodes.Count && i < touchs.Count; ++i) {
            touchs[i].gameObject.SetActive(true);
            touchs[i].img.sprite = Resources.Load<Sprite>(nodes[i].icon_file);
        }
    }
}
