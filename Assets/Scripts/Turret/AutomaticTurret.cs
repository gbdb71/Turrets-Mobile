using UnityEngine;

public class AutomaticTurret : BaseTurret
{
    [Label("Bullet Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(2, 40)] protected float _bulletSpeed;
    [SerializeField] protected BaseProjectile _projectilePrefab;

    protected override void Fire()
    {
        base.Fire();

        HomingProjectile projectile = Instantiate(_projectilePrefab, _shootPivot[ShootPivotIndex].transform.position, _shootPivot[ShootPivotIndex].transform.rotation) as HomingProjectile;
        projectile.Initialize(_shootPivot[ShootPivotIndex].transform.position, Vector3.zero, _damage, 0f);
        projectile.SetSpeed(_bulletSpeed);
        projectile.SetTarget(_currentTarget.transform);
        CheckPivotIndex();
    }

    private void CheckPivotIndex()
    {
        Debug.Log($"Do Shoot Pivot Index {ShootPivotIndex} | Pivot Lenght {_shootPivot.Length}");

        ShootPivotIndex += 1;
        if (ShootPivotIndex > _shootPivot.Length - 1)
            ShootPivotIndex = 0;

        Debug.Log($"Past Shoot Pivot Index {ShootPivotIndex} | Pivot Lenght {_shootPivot.Length}");
    }
}
