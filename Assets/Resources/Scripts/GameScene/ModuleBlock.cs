using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BLOCK_TYPE {
    BLOCK_OCEAN,
    BLOCK_SELF,
    BLOCK_OTHER,
    BLOCK_LAND
}
public class ModuleBlock : MonoBehaviour
{
    public BLOCK_TYPE type { get { return block_type; }
        set {
            block_type = value;
            if (null == ssprites) {
                ssprites = new List<Sprite>() { Resources.Load<Sprite>("Sprites/block_ocean")
                    , Resources.Load<Sprite>("Sprites/block_self")
                    , Resources.Load<Sprite>("Sprites/block_other")
                    , Resources.Load<Sprite>("Sprites/block_land") };
                sweights = new List<int>() { 8, 1, 5, 3 };
            }
            if ((int)(block_type) < ssprites.Count) {
                GetComponent<SpriteRenderer>().sprite = ssprites[(int)(block_type)];
            }
        } }

    public int weigtht { get { return sweights[(int)(type)]; } }

    static List<Sprite> ssprites;
    static List<int> sweights;

    BLOCK_TYPE block_type = BLOCK_TYPE.BLOCK_OCEAN;
    void Start()
    {
    }
}
