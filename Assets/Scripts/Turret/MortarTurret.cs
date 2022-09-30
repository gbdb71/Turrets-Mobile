using ToolBox.Pools;
using UnityEngine;

public class MortarTurret : BaseTurret
{
    [Label("Mortart Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(0.5f, 3f)] private float _projectileRadius = 1f;
    [SerializeField] private MortarShell _shellPrefab;

    private float _launchSpeed;
    private Vector3 _launchVelocity;
    private Vector3 _aimPos;

    protected override void Start()
    {
        base.Start();

        float x = _aim.AimDistance + 0.25001f;
        float y = -_shootPivot[ShootPivotIndex].position.y;
        _launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    protected override void Fire()
    {
        base.Fire();

        if (_launchVelocity != Vector3.zero)
        {
            MortarShell projectile = _shellPrefab.gameObject.Reuse<MortarShell>(_shootPivot[ShootPivotIndex].position, Quaternion.identity);
            projectile.Initialize(_shootPivot[ShootPivotIndex].position, _launchVelocity, _damage, _projectileRadius);
        }
    }
    protected override void Aim()
    {
        Vector3 launchPoint = _shootPivot[ShootPivotIndex].position;
        Vector3 targetPoint = _currentTarget.transform.position;
        targetPoint.y = 0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;
        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float g = 9.81f;
        float s = _launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        if (r >= 0)
        {
            _launchVelocity = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);

            _aimPos = targetPoint;
            _aimPos.y = tanTheta;

            _aim.SetIdle(false);
            _aim.SetAim(_aimPos);
        }
        else
        {
            _launchVelocity = Vector3.zero;
            _aim.SetIdle(true);
        }

    }
}
