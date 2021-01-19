using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModulePath : MonoBehaviour
{
    public GameObject path_template;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> pools = new List<GameObject>();
    void Start()
    {

    }
    void OnPersonPath(List<Vector3> paths)
    {
        foreach (var n in nodes)
        {
            n.SetActive(false);
            pools.Add(n);
        }
        nodes.Clear();
        if (0 < paths.Count)
        {
            GameObject obj = null;
            foreach (var p  in paths) {
                if (0 == pools.Count)
                {
                    obj = Instantiate(path_template, transform);
                }
                else {
                    obj = pools[0];
                    pools.RemoveAt(0);
                    obj.SetActive(true);
                    nodes.Add(obj);
                }
                obj.transform.position = p;
            }
        }
    }
}
