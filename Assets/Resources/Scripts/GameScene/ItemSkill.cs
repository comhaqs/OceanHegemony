using UnityEngine;
using System.Collections;

public class ItemSkill : Item
{
    public virtual void Start() {
    }
    public virtual void OnAction(Person person) {
        Destroy(gameObject);
    }
}
