using UnityEngine;
using System.Collections;

public class ItemSkill : Item
{
    public virtual void OnAction() {
        Destroy(gameObject);
    }
}
