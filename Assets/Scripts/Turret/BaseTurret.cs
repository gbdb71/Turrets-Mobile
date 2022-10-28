using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(TurretAim)), SelectionBase]
public abstract class BaseTurret : MonoBehaviour
{
    #region Serialized

    [Label("Damage", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] 
    [SerializeField, Range(1, 200)] protected float _damage;

    [Label("Shooting", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] 
    [SerializeField, Range(0f, 5f)] protected float _fireDelay;

    [SerializeField] protected Transform[] _shootPivot;

    [Label("Upgrade", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] 
    [SerializeField] private BaseTurret _nextGrade = default;

    [SerializeField] private ParticleSystem upgradeParticle;

    [Label("Selected settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] 
    [SerializeField] private Color _rangeColor;

    [SerializeField, ColorUsage(true, true)] private Color _selectedColor;

    [SerializeField] private Material _selectedMaterial;

    #endregion

    #region State

    private Material _defaultMaterial;
    private Renderer[] _renderers;
    protected TurretAim _aim;
    protected Enemy _currentTarget;
    protected int _currentShootPivot = 0;
    private int _abillitiesCount = 0;
    protected float _fireTimer = 0f;

    #endregion

    #region Public

    public bool CanUseAbillity => _abillitiesCount < 3;
    public BaseTurret NextGrade => _nextGrade;
    public TurretCanvas Canvas { get; private set; }
    public static List<BaseTurret> Turrets { get; private set; } = new List<BaseTurret>();

    #endregion
    
    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();
        Canvas = GetComponentInChildren<TurretCanvas>();
        _renderers = GetComponentsInChildren<MeshRenderer>();
        _defaultMaterial = _renderers[0].material;

        Turrets.Add(this);
    }

    protected virtual void Start()
    {
        if (Canvas != null && Canvas.Range != null)
        {
            Canvas.Range.color = _rangeColor;
        }
    }

    private void Update()
    {
        if (_currentTarget == null)
        {
            _currentTarget = FindTarget();
        }

        if (_currentTarget != null)
        {
            Aim();

            if (_currentTarget.IsDead || Vector3.Distance(_currentTarget.transform.position, transform.position) >
                _aim.AimDistance)
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
        }

        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        _aim.SetIdle(false);
    }

    private void OnDisable()
    {
        _aim.SetIdle(true);
    }

    private void OnDestroy()
    {
        Turrets.Remove(this);
    }

    #region Aim & Fire

    protected Enemy FindTarget()
    {
        if (TargetPoint.FillBuffer(transform.position, _aim.AimDistance))
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

    protected virtual void StopFire()
    {
    }

    protected virtual void Fire()
    {
        _fireTimer = _fireDelay;
    }

    protected virtual bool CanFire()
    {
        return _aim.IsAimed && _fireTimer <= 0f;
    }

    #endregion

    #region Visual

    public virtual void PlayUpgradeParticle()
    {
        if (upgradeParticle != null)
            upgradeParticle.Play();
    }

    public void SetSelected(bool selected)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (selected)
                _renderers[i].material = _selectedMaterial;
            else
                _renderers[i].material = _defaultMaterial;
        }

        SetActiveRangeImage(selected);
    }

    private void SetActiveRangeImage(bool enabled)
    {
        if (Canvas == null || Canvas.Range == null)
            return;

        Canvas.Range.gameObject.SetActive(enabled);
        Canvas.Range.transform.DOScale(new Vector3(_aim.AimDistance, _aim.AimDistance, _aim.AimDistance), .3f)
            .From(Vector3.zero).SetEase(Ease.OutBack);
    }

    #endregion

    #region Stats Control

    public void AddDamageValue(float percents)
    {
        if (percents < 0f)
            throw new ArgumentException(string.Format("{0} is not an positive number", percents),
                "value");

        _damage += _damage.Percent(percents);
        
        Canvas.AddStar();
        _abillitiesCount++;
    }

    public void DecreaseFireDelay(float percents)
    {
        if (percents < 0f)
            throw new ArgumentException(string.Format("{0} is not an positive number", percents),
                "value");

        _fireDelay -= _damage.Percent(percents);

        if (_fireDelay < 0.5f)
            _fireDelay = 0.5f;
        
        Canvas.AddStar();
        _abillitiesCount++;
    }

    #endregion
}