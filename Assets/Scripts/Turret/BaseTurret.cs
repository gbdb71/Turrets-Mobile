using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TurretAim)), SelectionBase]
public abstract class BaseTurret : MonoBehaviour
{
    [Label("Damage", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 200)] protected float _damage;

    [Label("Shooting", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.1f, 5f)] protected float _fireDelay;
    [SerializeField, Range(.5f, 5f)] protected float _reloadTime;
    [SerializeField] protected Transform _shootPivot;

    [Label("Taking", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _indicatorTransform = default;
    [SerializeField] private Image _indicatorFill = default;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] protected BaseProjectile _projectilePrefab;
    [Range(1, 100)]
    [SerializeField] protected int _chargedAmmoMax;
    [Range(1, 100)]
    [SerializeField] private int _ammoMax;

    [Label("Upgrade", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private BaseTurret _nextGrade = default;

    protected TurretAim _aim;
    protected Enemy _currentTarget;

    protected float _fireTimer = 0f;

    protected int _chargedAmmo = 0;
    protected int _ammo = 0;

    private Renderer[] _renderers;

    public Transform IndicatorTransform => _indicatorTransform;
    public Image IndicatorFill => _indicatorFill;
    public bool IsReloading { get; private set; }
    public BaseTurret NextGrade => _nextGrade;
    public Renderer[] Renderers => _renderers;


    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();
        _renderers = GetComponentsInChildren<Renderer>();

        _ammo = _ammoMax;
        _chargedAmmo = _chargedAmmoMax;
    }

    private void Update()
    {
        if (_currentTarget != null)
            Aim();

        if (_chargedAmmo > 0)
        {
            if (_currentTarget != null)
            {
                if (Vector3.Distance(_currentTarget.transform.position, transform.position) > _aim.AimDistance)
                {
                    _currentTarget = null;
                    return;
                }

                if (CanFire())
                {
                    Fire();
                }
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
        if (_currentTarget != null)
        {
            _aim.SetIdle(false);
            _aim.SetAim(_currentTarget.transform.position);
        }
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
        return _aim.IsAimed && _fireTimer <= 0f && !IsReloading && _chargedAmmo > 0;
    }

    protected Enemy FindTarget()
    {
        if (TargetPoint.FillBuffer(transform.localPosition, _aim.AimDistance))
        {
            return TargetPoint.RandomBuffered;
        }

        return null;
    }


    public void Charge(int amount)
    {
        _chargedAmmo += amount;
    }

}
