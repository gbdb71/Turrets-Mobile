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
    [SerializeField] protected Transform[] _shootPivot;

    [Label("Upgrade", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private BaseTurret _nextGrade = default;

    [Header("Selected settings")]
    private Renderer[] _renderers;
    [SerializeField] private Material mainMaterial;
    [SerializeField] private Material selectedMaterial;

    [Space]
    protected int _currentShootPivot = 0;

    protected TurretAim _aim;
    protected Enemy _currentTarget;

    protected float _fireTimer = 0f;

    public BaseTurret NextGrade => _nextGrade;
    public Renderer[] Renderers => _renderers;

    public float Damage
    {
        get
        {
            return _damage + _damage.Percent(SummableAbillity.GetValue(SummableAbillity.Type.TurretDamage));
        }
    }

    public static List<BaseTurret> Turrets { get; private set; } = new List<BaseTurret>();

    protected virtual void Awake()
    {
        _aim = GetComponent<TurretAim>();
        _renderers = GetComponentsInChildren<MeshRenderer>();

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

    private void Update()
    {
        if (_currentTarget == null)
        {
            _currentTarget = FindTarget();
        }

        if (_currentTarget != null)
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

    protected virtual void Fire()
    {
        _fireTimer = (_fireDelay - _fireDelay.Percent(SummableAbillity.GetValue(SummableAbillity.Type.TurretFire)));
    }

    protected virtual bool CanFire()
    {
        return _aim.IsAimed && _fireTimer <= 0f;
    }
}
