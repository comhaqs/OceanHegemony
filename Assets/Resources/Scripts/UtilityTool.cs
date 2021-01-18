using UnityEngine;
using System.Collections;

public class UtilityTool
{
    public static float block_size = 0.64f;


    public static void LogError(string msg) {
        Debug.LogError(msg);
    }

    public static void LogInfo(string msg)
    {
        Debug.Log(msg);
    }

    public static Vector3 NormalSize(Vector3 pos) {
        return new Vector3(((int)(pos.x + 0.5f * block_size) / block_size) * block_size, ((int)(pos.y + 0.5f * block_size) / block_size) * block_size, pos.z);
    }

    public enum Direction
    {
        DIRECTION_FORWARD,
        DIRECTION_RIGHT,
        DIRECTION_BACK,
        DIRECTION_LEFT
    }

    public static Direction ToDirection(Vector3 v) {
        var angle = Vector3.SignedAngle(v, Vector3.right, Vector3.forward);
        if (135.0f < angle || -135.0f > angle)
        {
            return Direction.DIRECTION_BACK;
        }
        else if (45.0f > angle)
        {
            return Direction.DIRECTION_LEFT;
        }
        else if (-45.0f > angle)
        {
            return Direction.DIRECTION_FORWARD;
        }
        else {
            return Direction.DIRECTION_RIGHT;
        }
    }
}
