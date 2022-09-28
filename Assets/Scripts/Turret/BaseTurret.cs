using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] protected Transform[] _shootPivot;

    [Label("Taking", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _indicatorTransform = default;
    [SerializeField] private Image _indicatorFill = default;

    [Label("Upgrade", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private BaseTurret _nextGrade = default;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [Range(1, 100)]
    [SerializeField] protected int _chargedAmmoMax;
    [Range(1, 100)]
    [SerializeField] private int _ammoMax;
    [SerializeField, Range(1, 100)] private int _ammoPerBox = 10;

    protected TurretAim _aim;
    protected Enemy _currentTarget;

    protected float _fireTimer = 0f;

    protected int _chargedAmmo = 0;
    protected int _ammo = 0;

    private Renderer[] _renderers;
    private AmmoBar _ammoBar;

    public Transform IndicatorTransform => _indicatorTransform;
    public Image IndicatorFill => _indicatorFill;
    public bool IsReloading { get; private set; }
    public BaseTurret NextGrade => _nextGrade;
    public Renderer[] Renderers => _renderers;
    public bool CanCharge { get { return _ammo < _ammoMax; } }
    public static List<BaseTurret> Turrets { get; private set; } = new List<BaseTurret>();
    public int ShootPivotIndex = 0;

    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();
        _renderers = GetComponentsInChildren<Renderer>();
        _ammoBar = GetComponentInChildren<AmmoBar>();

        _ammo = _ammoMax;
        _chargedAmmo = _chargedAmmoMax;

        Turrets.Add(this);
    }

    private void Start()
    {
        UpdateAmmoBar();
    }

    private void Update()
    {
        if (_currentTarget != null)
            Aim();

        if (_chargedAmmo > 0)
        {
            if (_currentTarget != null)
            {
                if (_currentTarget.IsDead || Vector3.Distance(_currentTarget.transform.position, transform.position) > _aim.AimDistance)
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

    private void OnEnable()
    {
        if (_ammoBar != null)
            _ammoBar.EnableBar();
    }

    private void OnDisable()
    {
        _aim.SetIdle(true);

        if (_ammoBar != null)
            _ammoBar.DisableBar();
    }

    private void OnDestroy()
    {
        Turrets.Remove(this);
    }

    protected Enemy FindTarget()
    {
        if (TargetPoint.FillBuffer(transform.localPosition, _aim.AimDistance))
        {
            return TargetPoint.RandomBuffered;
        }

        return null;
    }

    protected virtual void Aim()
    {
        if (_currentTarget != null)
        {
            _aim.SetIdle(false);
            _aim.SetAim(_currentTarget.transform.position);
        }
    }

    protected virtual void StopFire() { }
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

        UpdateAmmoBar();
    }

    private void UpdateAmmoBar()
    {
        if (_ammoBar != null)
            _ammoBar.ChangeValue(_chargedAmmo + _ammo, _chargedAmmoMax + _ammoMax);
    }

    protected virtual bool CanFire()
    {
        return _aim.IsAimed && _fireTimer <= 0f && !IsReloading && _chargedAmmo > 0;
    }

    public void Charge()
    {
        _ammo += _ammoPerBox;

        if (_ammo > _ammoMax)
            _ammo = _ammoMax;

        UpdateAmmoBar();
    }
}
