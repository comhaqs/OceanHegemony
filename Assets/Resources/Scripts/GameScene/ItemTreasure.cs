using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTreasure : Item
{
    public List<Item> item_templates;
    [HideInInspector]
    public List<Item> items = new List<Item>();
    void Start()
    {
        var count = UnityEngine.Random.Range(item_templates.Count / 2, item_templates.Count);
        for (int i = 0; i < count; ++i) {
            var item = Instantiate(item_templates[UnityEngine.Random.Range(0, item_templates.Count - 1)], transform);
            items.Add(item);
        }
    }
}
