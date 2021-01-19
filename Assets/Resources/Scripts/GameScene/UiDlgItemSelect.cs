using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiDlgItemSelect : MonoBehaviour
{
    public GameObject root;
    public List<UiTouch> package_person;
    public List<UiTouch> package_treasure;

    ItemTreasure treasure;
    Person person;
    void Start()
    {
        foreach (var t in package_person) {
            t.bt.onClick.AddListener(()=> {
                if (null == treasure || null == person) {
                    return;
                }
                if (treasure.item_max <= treasure.items.Count) {
                    return;
                }
                for (int i = 0; i < person.items.Count; ++i) {
                    if (person.items[i] == t) {
                        treasure.items.Add(person.items[i]);
                        person.items.RemoveAt(i);
                        UpdateUi();
                        return;
                    }
                }
            });
        }
        foreach (var t in package_treasure)
        {
            t.bt.onClick.AddListener(() =>
            {
                if (null == treasure || null == person)
                {
                    return;
                }
                if (person.item_max <= person.items.Count)
                {
                    return;
                }
                for (int i = 0; i < treasure.items.Count; ++i)
                {
                    if (treasure.items[i] == t)
                    {
                        person.items.Add(treasure.items[i]);
                        treasure.items.RemoveAt(i);
                        UpdateUi();
                        return;
                    }
                }
            });
        }
        MessageManager.GetInstance().Add<InfoParam2<GameObject, GameObject>>("ui_item_select", OnItemSelect, gameObject);
    }

    void OnItemSelect(InfoParam2<GameObject, GameObject> param)
    {
        root.SetActive(true);
        treasure = param.param1.GetComponent<ItemTreasure>();
        person = param.param2.GetComponent<Person>();
        UpdateUi();
    }

    void OnDlgClose() {
        root.SetActive(false);
        person = null;
        treasure = null;
    }

    void UpdateUi() {
        for (int i = 0; i < package_person.Count && i < person.items.Count; ++i) {
            package_person[i].img.sprite = person.items[i].GetComponent<SpriteRenderer>().sprite;
        }
        for (int i = 0; i < package_treasure.Count && i < treasure.items.Count; ++i)
        {
            package_treasure[i].img.sprite = treasure.items[i].GetComponent<SpriteRenderer>().sprite;
        }
    }
}
