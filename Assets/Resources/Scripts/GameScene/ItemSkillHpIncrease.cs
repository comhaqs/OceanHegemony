using UnityEngine;
using System.Collections;

public class ItemSkillHpIncrease : ItemSkill
{
    public int hp_min = 20;
    public int hp_max = 40;
    int hp = 0;

    public override void Start()
    {
        base.Start();
        hp = UnityEngine.Random.Range(hp_min, hp_max);
    }

    public override void OnAction(Person person)
    {
        person.hp += hp;
        base.OnAction(person);
    }
}
