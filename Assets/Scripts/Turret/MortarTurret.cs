using UnityEngine;

public class MortarTurret : BaseTurret<MortarShell>
{
    [Header("Mortar")]
    [SerializeField] private Transform _mortar;
    [SerializeField, Range(0.5f, 3f)]
    private float _projectileRadius = 1f;
    [SerializeField, Range(1f, 100f)]
    private float _projectileDamage = 10f;

    private float _launchSpeed;

    private void Start()
    {
        float x = _aim.AimDistance + 0.25001f;
        float y = -_shootPivot.position.y;
        _launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    protected override void Fire()
    {
        base.Fire();

        Vector3 launchPoint = _shootPivot.position;
        Vector3 targetPoint = _currentTarget.Transform.position;
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
            MortarShell projectile = Instantiate(_projectilePrefab, _shootPivot.position, Quaternion.identity);

            projectile.Initialize(launchPoint, new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y), _projectileRadius, _projectileDamage);
        }
    }
}
