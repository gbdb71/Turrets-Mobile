using UnityEngine;


public class TargetPoint
{
    const int enemyLayerMask = 1 << 8;

    static Collider[] buffer = new Collider[100];

    public static int BufferedCount { get; private set; }

    public static Enemy RandomBuffered =>
        GetBuffered(Random.Range(0, BufferedCount));

    public static bool FillBuffer(Vector3 position, float range)
    {
        Vector3 top = position;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(
            position, top, range, buffer, enemyLayerMask
        );
        return BufferedCount > 0;
    }

    public static Enemy GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<Enemy>();
        Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
        return target;
    }
}
