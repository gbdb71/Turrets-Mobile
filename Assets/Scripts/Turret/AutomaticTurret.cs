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

    protected override void Awake()
    {
        base.Awake();
    }
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

        if (_muzzleParticles[_currentShootPivot] != null && _muzzleParticles.Count > 0)
            _muzzleParticles[_currentShootPivot].Play();

        _shootPivot[_currentShootPivot].parent.DOLocalMoveZ(gunMove.y, moveDuration.x).OnComplete(() =>
        {
            _shootPivot[_currentShootPivot].parent.DOLocalMoveZ(gunMove.x, moveDuration.y);
        });

        HomingProjectile projectile = _projectilePrefab.gameObject.Reuse<BaseProjectile>(_shootPivot[_currentShootPivot].transform.position, _shootPivot[_currentShootPivot].transform.rotation) as HomingProjectile;
        projectile.Initialize(_shootPivot[_currentShootPivot].transform.position, Vector3.zero, _damage, 0f);
        projectile.SetSpeed(_bulletSpeed);
        projectile.SetY(false);
        projectile.SetTarget(_currentTarget.transform);

        ChangePivotIndex();
    }

    private void ChangePivotIndex()
    {
        _currentShootPivot += 1;

        if (_currentShootPivot > _shootPivot.Length - 1)
            _currentShootPivot = 0;
    }
}

