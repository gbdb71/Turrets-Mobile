using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TurretAim))]
public abstract class BaseTurret : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField, Range(1, 200)] protected float _damage;
    
    [Header("Shooting")]
    [SerializeField, Range(.1f, 5f)] protected float _fireDelay;
    [SerializeField, Range(.5f, 5f)] protected float _reloadTime;
    [SerializeField] protected Transform _shootPivot;

    [Space]

    [Header("Take")]
    [SerializeField] private Transform _indicatorTransform;
    [SerializeField] private Image _indicatorFill;

    [Space]

    [Header("Ammunition")]
    [SerializeField] protected Rigidbody _projectilePrefab;
    [Range(1, 100)]
    [SerializeField] protected int _chargedAmmoMax;
    [Range(1, 100)]
    [SerializeField] private int _ammoMax;

    protected TurretAim _aim;
    protected IDamagable _currentTarget;

    protected float _fireTimer = 0f;

    protected int _chargedAmmo = 0;
    protected int _ammo = 0;

    public Transform IndicatorTransform => _indicatorTransform;
    public Image IndicatorFill => _indicatorFill;
    public bool IsReloading { get; private set; }


    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();

        _ammo = _ammoMax;
        _chargedAmmo = _chargedAmmoMax;
    }

    private void Update()
    {
        if (_chargedAmmo > 0)
        {
            if (_currentTarget != null)
            {
                if (Vector3.Distance(_currentTarget.Transform.position, transform.position) > _aim.AimDistance)
                {
                    _currentTarget = null;
                    return;
                }

                if (CanFire())
                {
                    Fire();
                }

                Aim();
            }
            else
            {
                StopFire();

                _aim.SetIdle(true);
                _currentTarget = FindTarget();
            }
        }
        else
        {
            StopFire();

            if (_ammo == 0)
            {
                _aim.SetIdle(true);
            }
            else if (!IsReloading)
            {
                StartCoroutine(nameof(Reload));
            }
        }

        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;
    }

    protected virtual void StopFire() { }

    private void OnDisable()
    {
        _aim.SetIdle(true);
    }

    protected virtual void Aim()
    {
        _aim.SetIdle(false);
        _aim.SetAim(_currentTarget.Transform.position);
    }

    protected virtual IEnumerator Reload()
    {
        IsReloading = true;

        yield return new WaitForSeconds(_reloadTime);

        if (_ammo >= _chargedAmmoMax)
        {
            _ammo -= _chargedAmmoMax;
            _chargedAmmo += _chargedAmmoMax;
        }
        else
        {
            _chargedAmmo = _ammo;
            _ammo = 0;
        }

        IsReloading = false;
    }

    protected virtual void Fire()
    {
        _fireTimer = _fireDelay;
        _chargedAmmo -= 1;
    }

    protected virtual bool CanFire()
    {
        return _currentTarget != null && _aim.IsAimed && _fireTimer <= 0f && !IsReloading && _chargedAmmo > 0;
    }

    protected IDamagable FindTarget()
    {
        if (TargetPoint.FillBuffer(transform.localPosition, _aim.AimDistance))
        {
            return TargetPoint.RandomBuffered;
        }

        return null;
    }

}
