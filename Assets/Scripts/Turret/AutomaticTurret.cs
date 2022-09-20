using UnityEngine;

public class AutomaticTurret : BaseTurret
{
    [Label("Bullet Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(2, 40)] protected float _bulletSpeed;
    [SerializeField] protected BaseProjectile _projectilePrefab;

    protected override void Fire()
    {
        base.Fire();

        HomingProjectile projectile = Instantiate(_projectilePrefab, _shootPivot.transform.position, _shootPivot.transform.rotation) as HomingProjectile;
        projectile.Initialize(_shootPivot.transform.position, Vector3.zero, _damage, 0f);

        projectile.SetSpeed(_bulletSpeed);
        projectile.SetTarget(_currentTarget.transform);
    }
}
