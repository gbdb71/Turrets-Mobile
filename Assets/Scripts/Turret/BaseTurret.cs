using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(TurretAim)), SelectionBase]
public abstract class BaseTurret : MonoBehaviour
{
    [Label("Damage", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 200)] protected float _damage;

    [Label("Shooting", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(0f, 5f)] protected float _fireDelay;
    [SerializeField, Range(.5f, 5f)] protected float _reloadTime;
    [SerializeField] protected Transform[] _shootPivot;

    [Label("Upgrade", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private BaseTurret _nextGrade = default;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [Range(1, 100)]
    [SerializeField] protected int _chargedAmmoMax;
    [Range(1, 100)]
    [SerializeField] protected int _ammoMax;
    [SerializeField, Range(1, 100)] private int _ammoPerBox = 10;

    [Header("Selected settings")]
    private Renderer[] _renderers;
    [SerializeField] private Material mainMaterial;
    [SerializeField] private Material selectedMaterial;

    [Space]
    protected int _currentShootPivot = 0;

    protected BaseBar _baseBar;
    protected TurretAim _aim;
    protected Enemy _currentTarget;

    protected float _fireTimer = 0f;

    protected int _chargedAmmo = 0;
    protected int _ammo = 0;

    public BaseTurret NextGrade => _nextGrade;
    public Renderer[] Renderers => _renderers;

    public int Ammo => _ammo;
    public float Damage
    {
        get
        {
            return _damage + _damage.Percent(SummableAbillity.GetValue(SummableAbillity.Type.TurretDamage));
        }
    }
    public bool IsReloading { get; private set; }
    public bool CanCharge { get { return _ammo < _ammoMax; } }

    public static List<BaseTurret> Turrets { get; private set; } = new List<BaseTurret>();

    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();
        _renderers = GetComponentsInChildren<MeshRenderer>();
        _baseBar = GetComponentInChildren<BaseBar>();

        _ammo = _ammoMax;
        _chargedAmmo = _chargedAmmoMax;

        Turrets.Add(this);
    }

    public virtual void PlayUpgradeParticle() { }

    public void SetSelected(bool selected)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = selected ? selectedMaterial : mainMaterial;
        }
    }

    protected virtual void Start()
    {
        UpdateAmmoBar();
    }

    private void Update()
    {
        if (_chargedAmmo > 0 && _currentTarget != null)
        {
            Aim();

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
            if (_currentTarget == null)
            {
                _currentTarget = FindTarget();
            }

            StopFire();

            if (_chargedAmmo == 0 && _ammo > 0 && !IsReloading)
            {
                StartCoroutine(nameof(Reload));
            }
        }

        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        if (_baseBar != null)
            _baseBar.EnableBar();

        _aim.SetIdle(false);
    }

    private void OnDisable()
    {
        _aim.SetIdle(true);

        if (_baseBar != null)
            _baseBar.DisableBar();
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
            _aim.SetAim(_currentTarget.transform.position);
        }
    }

    protected virtual void StopFire() { }
    protected virtual IEnumerator Reload()
    {
        IsReloading = true;

        float reloadTime = _reloadTime - _reloadTime.Percent(SummableAbillity.GetValue(SummableAbillity.Type.TurretReload));
        _baseBar.EnableReloadIndicator(reloadTime);

        yield return new WaitForSeconds(reloadTime);

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
        _fireTimer = (_fireDelay - _fireDelay.Percent(SummableAbillity.GetValue(SummableAbillity.Type.TurretFire)));
        _chargedAmmo -= 1;

        UpdateAmmoBar();
    }

    protected void UpdateAmmoBar()
    {
        if (_baseBar != null)
            _baseBar.ChangeValue(_chargedAmmo + _ammo, _chargedAmmoMax + _ammoMax);
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
