using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Person : MonoBehaviour
{
    public string nm { get { return self_name; } set { self_name = value; if (null != text_name) { text_name.text = self_name; } } }
    public int attack = 0;
    public int hp { get { return self_hp; } set { var hp_old = self_hp; self_hp = value; if (0 > self_hp) { self_hp = 0; } GetComponent<ModuleDigitDisplay>().AddDigit( (hp_old - self_hp).ToString()); } }
    public List<Item> items = new List<Item>(4);
    public int camp = 0;
    public TextMeshPro text_name;
    [HideInInspector]
    public int power { get { return self_power; } set { self_power = value; } }

    protected int self_power = 1000;
    protected string self_name;
    protected int self_hp = 100;
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
