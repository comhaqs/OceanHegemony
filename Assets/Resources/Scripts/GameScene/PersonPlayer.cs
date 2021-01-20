using UnityEngine;
using System.Collections;


public class PersonPlayer : Person
{
    void OnEnable()
    {
        MessageManager.GetInstance().Add<GameObject>("person_touch_item", OnPersonTouchItem, gameObject);
        MessageManager.GetInstance().Add<GameObject>("person_touch_enemy", OnPersonTouchEnemy, gameObject);
        MessageManager.GetInstance().Add<InfoParam1<Person>>("person_player", OnPersonPlayer, gameObject);
    }

    void OnPersonTouchItem(GameObject obj)
    {
        MessageManager.GetInstance().Notify("ui_item_select", new InfoParam2<GameObject, GameObject>() { param1 = obj, param2 = gameObject});
    }

    void OnPersonTouchEnemy(GameObject obj)
    {
        var enemy = obj.GetComponent<Person>();
        var direction = UtilityTool.ToDirection(enemy.transform.position - transform.position);
        if (UtilityTool.Direction.DIRECTION_FORWARD == direction)
        {
            enemy.hp -= attack;
        }
        else if (UtilityTool.Direction.DIRECTION_LEFT == direction || UtilityTool.Direction.DIRECTION_RIGHT == direction)
        {
            enemy.hp -= (int)(0.7f * attack);
        }
        else {

        }
    }

    void OnPersonPlayer(InfoParam1<Person> param) {
        param.param1 = this;
    }
}
