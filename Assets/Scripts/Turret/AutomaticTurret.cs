using UnityEngine;
using DG.Tweening;
using ToolBox.Pools;
using System.Collections.Generic;

public class AutomaticTurret : BaseTurret
{
    [Label("Bullet Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(2, 40)] protected float _bulletSpeed;
    [SerializeField] protected BaseProjectile _projectilePrefab;

    [SerializeField] private Vector2 gunMove = new Vector2(0.001f, -0.001f);
    [SerializeField] private Vector2 moveDuration = new Vector2(0.075f, 0.125f);

    [SerializeField] private List<ParticleSystem> _muzzleParticles;

    protected override void Start()
    {
        base.Start();
        
        for (int i = 0; i < _shootPivot.Length; i++)
        {
            _muzzleParticles.Add(_shootPivot[i].parent.gameObject.GetComponentInChildren<ParticleSystem>());
        }
    }

    protected override void Fire()
    {
        base.Fire();

        if (_muzzleParticles[ShootPivotIndex] != null && _muzzleParticles.Count > 0)
            _muzzleParticles[ShootPivotIndex].Play();

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

