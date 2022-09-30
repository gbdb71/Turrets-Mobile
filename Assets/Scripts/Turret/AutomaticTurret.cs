using UnityEngine;
using DG.Tweening;
using ToolBox.Pools;

public class AutomaticTurret : BaseTurret
{
    [Label("Bullet Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(2, 40)] protected float _bulletSpeed;
    [SerializeField] protected BaseProjectile _projectilePrefab;

    [SerializeField] private Vector2 gunMove = new Vector2(0.001f, -0.001f);
    [SerializeField] private Vector2 moveDuration = new Vector2(0.075f, 0.125f);

    protected override void Start()
    {
        base.Start();
    }

    protected override void Fire()
    {
        base.Fire();

        _shootPivot[ShootPivotIndex].parent.DOLocalMoveZ(gunMove.y, moveDuration.x).OnComplete(() =>
        {
            _shootPivot[ShootPivotIndex].parent.DOLocalMoveZ(gunMove.x, moveDuration.y);
        });

        HomingProjectile projectile = _projectilePrefab.gameObject.Reuse<BaseProjectile>(_shootPivot[ShootPivotIndex].transform.position, _shootPivot[ShootPivotIndex].transform.rotation) as HomingProjectile;
        projectile.Initialize(_shootPivot[ShootPivotIndex].transform.position, Vector3.zero, _damage, 0f);
        projectile.SetSpeed(_bulletSpeed);
        projectile.SetTarget(_currentTarget.transform);

        ChangePivotIndex();
    }

    private void ChangePivotIndex()
    {
        ShootPivotIndex += 1;

        if (ShootPivotIndex > _shootPivot.Length - 1)
            ShootPivotIndex = 0;
    }
}

