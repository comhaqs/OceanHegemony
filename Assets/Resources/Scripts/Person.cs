using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
    public string nm;
    public int attack = 0;
    public int hp = 100;
    public List<Item> items = new List<Item>();
    public int item_max = 4;
    [HideInInspector]
    public int power { get { return self_power; } set { self_power = value; } }

    protected int self_power = 1000;
    public virtual void Start()
    {

    }
}
