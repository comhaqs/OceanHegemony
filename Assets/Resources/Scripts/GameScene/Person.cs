using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
    public string nm;
    public int attack = 0;
    public int hp = 100;
    public List<Item> items = new List<Item>(4);
    public int camp = 0;
    [HideInInspector]
    public int power { get { return self_power; } set { self_power = value; } }

    protected int self_power = 1000;
    public virtual void Start()
    {

    }

    protected virtual void DoAttack(Person enemy)
    {
        var direction = UtilityTool.ToDirection(enemy.transform.position - transform.position);
        if (UtilityTool.Direction.DIRECTION_FORWARD == direction)
        {
            enemy.hp -= attack;
        }
        else if (UtilityTool.Direction.DIRECTION_LEFT == direction || UtilityTool.Direction.DIRECTION_RIGHT == direction)
        {
            enemy.hp -= (int)(0.7f * attack);
        }
        else
        {

        }
    }
}
