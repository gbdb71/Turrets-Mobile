
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static List<Vector3> FromVec2Int(this List<Vector2Int> list)
    {
        List<Vector3> result = new List<Vector3>();

        for (int i = 0; i < list.Count; i++)
        {
            result.Add(new Vector3(list[i].x, 0f, list[i].y));
        }

        return result;
    }
}

