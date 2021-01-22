using UnityEngine;
using System.Collections;

public class ItemSkillMpSpeed : ItemSkill
{

    public int mp_speed_min = 5;
    public int mp_speed_max = 15;
    public float time_min = 20.0f;
    public float time_max = 40.0f;
    int mp_speed = 0;
    float time = 0.0f;

    public override void Start()
    {
        base.Start();
        mp_speed = UnityEngine.Random.Range(mp_speed_min, mp_speed_max);
        time = UnityEngine.Random.Range(time_min, time_max);
    }

    public override void OnAction(Person person)
    {
        StartCoroutine(OnMpSpeed(person));
    }

    IEnumerator OnMpSpeed(Person person) {
        person.mp_speed += mp_speed;
        yield return new WaitForSeconds(time);
        person.mp_speed -= mp_speed;
        base.OnAction(person);
    }
}
