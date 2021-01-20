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

    private void OnEnable()
    {
        MessageManager.GetInstance().Add<Item>("ui_item_click", OnUiItemClick, gameObject);
    }

    void Start()
    {
        for (int i = 0; i < package_person.Count; ++i) {
            var select_package = package_person[i];
            select_package.bt.onClick.AddListener(()=>{
                if (null == treasure || null == person)
                {
                    return;
                }
                for (int j = 0; j < person.items.Count; ++j) {
                    if (person.items[j] == select_package.item) {
                        treasure.items.Add(person.items[j]);
                        person.items.RemoveAt(j);
                        UpdateUi();
                        return;
                    }
                }
            });
        }
        for (int i = 0; i < package_treasure.Count; ++i)
        {
            var select_package = package_treasure[i];
            select_package.bt.onClick.AddListener(() => {
                if (null == treasure || null == person)
                {
                    return;
                }
                for (int j = 0; j < treasure.items.Count; ++j)
                {
                    if (treasure.items[j] == select_package.item)
                    {
                        person.items.Add(treasure.items[j]);
                        treasure.items.RemoveAt(j);
                        UpdateUi();
                        return;
                    }
                }
            });
        }
    }

    public void OnDlgClose() {
        root.SetActive(false);
        person = null;
        treasure = null;
    }

    void UpdateUi() {
        foreach (var n in package_person) {
            n.gameObject.SetActive(false);
        }
        for (int i = 0; i < package_person.Count && i < person.items.Count; ++i) {
            package_person[i].gameObject.SetActive(true);
            package_person[i].img.sprite = Resources.Load<Sprite>(person.items[i].icon_file);
            package_person[i].item = person.items[i];
        }
        foreach (var n in package_treasure)
        {
            n.gameObject.SetActive(false);
        }
        for (int i = 0; i < package_treasure.Count && i < treasure.items.Count; ++i)
        {
            package_treasure[i].gameObject.SetActive(true);
            package_treasure[i].img.sprite = Resources.Load<Sprite>(treasure.items[i].icon_file);
            package_treasure[i].item = treasure.items[i];
        }
        MessageManager.GetInstance().Notify("ui_person_skill_update");
    }

    void OnUiItemClick(Item param) {
        var info_treasure = param.GetComponent<ItemTreasure>();
        if (null == info_treasure)
        {
            UtilityTool.LogError("类型转换失败[ItemTreasure]");
            return;
        }
        var info_player = new InfoParam1<Person>();
        MessageManager.GetInstance().Notify("person_player", info_player);
        if (null == info_player.param1) {
            return;
        }
        person = info_player.param1;
        treasure = info_treasure;
        root.SetActive(true);
        UpdateUi();
    }
}
