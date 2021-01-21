using UnityEngine;
using System.Collections;


public class PersonPlayer : Person
{
    public override void Start()
    {
        nm = "自己";
        base.Start();
        MessageManager.GetInstance().Notify("ui_person_skill_update");
        MessageManager.GetInstance().Notify("map_node_update", gameObject);
        MessageManager.GetInstance().Notify("ui_player_info_update", this as Person);
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

    protected override void OnIncreaseMp()
    {
        var old_mp = mp;
        base.OnIncreaseMp();
        if (old_mp != mp) {
            MessageManager.GetInstance().Notify("ui_player_mp_update", 1.0f * mp / mp_max);
        }
    }
}
