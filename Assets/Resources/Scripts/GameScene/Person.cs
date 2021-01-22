using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Person : MonoBehaviour
{
    public string nm { get { return self_name; } set { self_name = value; if (null != text_name) { text_name.text = self_name; } } }
    public int attack = 0;
    public int hp { get { return self_hp; } set { OnHpChange(value); } }
    public int mp = 100;
    public List<Item> items = new List<Item>();
    public int item_max = 4;
    public int camp = 0;
    public TextMeshPro text_name;
    public int hp_max = 100;
    public int mp_max = 100;
    public int mp_speed = 5;
    [HideInInspector]
    public bool flag_show = true;
    
    protected string self_name;
    protected int self_hp = 100;
    public virtual void Start()
    {
        StartCoroutine(IncreaseMp());
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

    IEnumerator IncreaseMp() {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            OnIncreaseMp();
        }
    }

    protected virtual void OnIncreaseMp() {
        mp += mp_speed;
        if (0 > mp) {
            mp = 0;
        }
        if (mp > mp_max)
        {
            mp = mp_max;
        }
    }

    protected virtual void OnHpChange(int hp_new)
    {
        var hp_old = self_hp;
        self_hp = hp_new;
        if (0 > self_hp)
        {
            self_hp = 0;
        }
        if (hp_max < self_hp) {
            self_hp = hp_max;
        }
        if (0 < hp_old - self_hp)
        {
            GetComponent<ModuleDigitDisplay>().AddDigit(string.Format("{0}", self_hp - hp_old));
        }
        else if (0 == hp_old - self_hp) {
            GetComponent<ModuleDigitDisplay>().AddDigit(string.Format("0"));
        }
        else
        {
            GetComponent<ModuleDigitDisplay>().AddDigit(string.Format("+{0}", hp_old - self_hp));
        }
        
    }
}
