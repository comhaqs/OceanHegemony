using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSkillEnemyAttack : ItemSkill
{
    public int range = 5;
    public ModuleBullet bullet_template;
    public int attack_min = 10;
    public int attack_max = 20;
    public override void OnAction(Person person)
    {
        var param = new InfoParam3<List<Person>, Vector3, int>() { param2 = transform.position, param3 = range };
        MessageManager.GetInstance().Notify("map_person_range", param);
        var persons = new List<Person>();
        foreach (var p in param.param1)
        {
            if (p.camp != person.camp)
            {
                persons.Add(p);
            }
        }
        foreach (var p in persons) {
            var bullet = Instantiate(bullet_template);
            bullet.target = p;
            bullet.attack = UnityEngine.Random.Range(attack_min, attack_max);
        }
        base.OnAction(person);
    }
}
