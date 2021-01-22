using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public string icon_file;
    public int mp = 0;

    [HideInInspector]
    public bool flag_show = true;
}
