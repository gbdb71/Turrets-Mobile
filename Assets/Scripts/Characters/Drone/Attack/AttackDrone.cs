using Characters.Drone.Attack;
using ToolBox.Pools;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TurretAim))]
public class AttackDrone : BaseDrone<AttackStateMachine>
{
    [SerializeField] private float _damage = 2f;
    [SerializeField] private float _fireDelay = .3f;
    [SerializeField] private Transform _shootPivot;
    [SerializeField] private HomingProjectile _projectilePrefab;

    private TurretAim _aim;

    public Player Player { get; private set; }
    public Enemy CurrentTarget { get; private set; }

    [Inject]
    private void Construct(Player player)
    {
        Player = player;
    }

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new AttackStateMachine(this);
        _aim = GetComponent<TurretAim>();
    }

    private void OnEnable()
    {
        _aim.SetIdle(false);
    }

    private void OnDisable()
    {
        _aim.SetIdle(true);
    }

    private float _fireTimer = 0f;

    protected override void UpdateLogic()
    {
        if (CurrentTarget == null)
        {
            CurrentTarget = FindTarget();
            return;
        }
        else if(Vector3.Distance(CurrentTarget.transform.position, transform.position) > _aim.AimDistance)
        {
            CurrentTarget = null;
            return;
        }

        _aim.SetAim(CurrentTarget.transform.position);

        if(_aim.IsAimed && _fireTimer <= 0)
        {
            Fire();

            _fireTimer = _fireDelay;
        }

        if (_fireTimer >= 0f)
            _fireTimer -= Time.deltaTime;
    }

    private void Fire()
    {
        HomingProjectile projectile = _projectilePrefab.gameObject.Reuse<HomingProjectile>(_shootPivot.transform.position, _shootPivot.transform.rotation);
        projectile.Initialize(_shootPivot.transform.position, Vector3.zero, _damage, 0f);
        projectile.SetSpeed(12f);
        projectile.SetY(true);
        projectile.SetTarget(CurrentTarget.transform);
    }

    protected Enemy FindTarget()
    {
        if (TargetPoint.FillBuffer(transform.localPosition, _aim.AimDistance))
        {
            return TargetPoint.RandomBuffered;
        }

        return null;
    }
}
