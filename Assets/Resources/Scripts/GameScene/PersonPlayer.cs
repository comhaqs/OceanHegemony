﻿using UnityEngine;
using System.Collections;


public class PersonPlayer : Person
{
    public override void Start()
    {
        base.Start();
        MessageManager.GetInstance().Notify("ui_person_skill_update");
        MessageManager.GetInstance().Notify("map_node_update", gameObject);
    }
    void OnEnable()
    {
        MessageManager.GetInstance().Add<GameObject>("person_touch_item", OnPersonTouchItem, gameObject);
        MessageManager.GetInstance().Add<Person>("ui_person_click", OnUiPersonClick, gameObject);
        MessageManager.GetInstance().Add<InfoParam1<Person>>("person_player", OnPersonPlayer, gameObject);
    }

    void OnPersonTouchItem(GameObject obj)
    {
        MessageManager.GetInstance().Notify("ui_item_select", new InfoParam2<GameObject, GameObject>() { param1 = obj, param2 = gameObject});
    }

    void OnUiPersonClick(Person enemy)
    {
        DoAttack(enemy);
    }

    void OnPersonPlayer(InfoParam1<Person> param) {
        param.param1 = this;
    }
}
