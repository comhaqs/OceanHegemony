using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PathType {
    PATH_ALLOW,
    PATH_FORBID
}
public class ModulePath : MonoBehaviour
{
    public PathType type { get { return self_type; }  set { self_type = value; OnTypeChange(); } }
    public int weight = 0;

    PathType self_type = PathType.PATH_FORBID;
    void OnTypeChange() {
        string file = "";
        if (PathType.PATH_ALLOW == self_type) {
            file = "Sprites/path_allow";
        } else if (PathType.PATH_FORBID == self_type) {
            file = "Sprites/path_forbit";
        }
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(file);
    }
}
