using System;
using UnityEngine;

public static class Extensions
{
    public static float Percent(this float number, float percent)
    {
        Debug.Log((float)(number * percent / 100) + "2");
        return (float)(number * percent / 100);
    }
}
