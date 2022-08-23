using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TurretAim))]
public abstract class BaseTurret : MonoBehaviour
{
    [Header("Shooting")]
    [Range(.1f, 5f)]
    [SerializeField] protected float _fireDelay;
    [Range(.5f, 5f)]
    [SerializeField] protected float _reloadTime;
    [SerializeField] protected Transform _shootPivot;

    [Space]

    [Header("Ammunition")]
    [SerializeField] protected Rigidbody _ammunitionPrefab;
    [Range(1, 100)]
    [SerializeField] protected int _chargedAmmoMax;
    [Range(1, 100)]
    [SerializeField] private int _ammoMax;

    [Space]

    [Header("Field Of View")]
    [SerializeField] protected LayerMask _mask;

    protected TurretAim _aim;
    protected IDamagable _currentTarget;

    protected float _fireTimer = 0f;

    protected int _chargedAmmo = 0;
    protected int _ammo = 0;

    public bool IsReloading { get; private set; }

    private void Awake()
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
                _aim.SetIdle(true);
                _currentTarget = FindTarget();
            }
        }
        else
        {
            if(_ammo == 0)
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

    protected abstract void Fire();

    protected virtual bool CanFire()
    {
        return _currentTarget != null && _aim.IsAimed && _fireTimer <= 0f && !IsReloading && _chargedAmmo > 0;
    }

    protected IDamagable FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _aim.AimDistance, _mask);

        IDamagable target = null;
        float dist = Mathf.Infinity;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out IDamagable damagable))
            {
                if (Vector3.Distance(transform.position, colliders[i].transform.position) < dist)
                {
                    target = damagable;
                }
            }
        }

        return target;
    }

}
