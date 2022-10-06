using System;

public static class Extensions
{
    public static float Percent(this float number, float percent)
    {
        return (float)(number * percent / 100);
    }
}
