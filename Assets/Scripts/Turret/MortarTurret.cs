using ToolBox.Pools;
using UnityEngine;

public class MortarTurret : BaseTurret
{
    [Label("Mortart Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(0.5f, 3f)] private float _projectileRadius = 1f;
    [SerializeField] private MortarShell _shellPrefab;

    [SerializeField] private ParticleSystem _muzzleParticle;

    private float _launchSpeed;
    private Vector3 _launchVelocity;
    private Vector3 _aimPos;
    private Vector3[] _velocitySolutions = new Vector3[2];

    protected override void Start()
    {
        base.Start();

        float x = _aim.AimDistance + 0.25001f;
        float y = -_shootPivot[_currentShootPivot].position.y;
        _launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    protected override void Fire()
    {
        base.Fire();

        if (_launchVelocity != Vector3.zero)
        {
            if (_muzzleParticle != null)
                _muzzleParticle.Play();

            MortarShell projectile = _shellPrefab.gameObject.Reuse<MortarShell>(_shootPivot[_currentShootPivot].position, Quaternion.identity);
            projectile.Initialize(_shootPivot[_currentShootPivot].position, _launchVelocity, _damage, _projectileRadius);
        }
    }


    protected override void Aim()
    {
        Vector3 launchPoint = _shootPivot[_currentShootPivot].position;
        Vector3 targetPoint = _currentTarget.transform.position;
        targetPoint.y = 0f;

        float g = 9.81f;
        Vector2 dir = (Vector2)(targetPoint - launchPoint);

        int numSolutions = fts.solve_ballistic_arc(launchPoint, _launchSpeed, targetPoint, _currentTarget.Velocity, g, out _velocitySolutions[0], out _velocitySolutions[1]);

        if (numSolutions > 0)
        {
            _launchVelocity = _velocitySolutions[1];

            float x = dir.magnitude;
            float y = -launchPoint.y;

            float s = _launchSpeed;
            float s2 = s * s;

            float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
            float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);

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
