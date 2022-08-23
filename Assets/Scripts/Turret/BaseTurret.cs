using UnityEngine;

[RequireComponent(typeof(TurretAim))]
public abstract class BaseTurret : MonoBehaviour
{

    [Header("Shooting")]
    [Range(.1f, 5f)]
    [SerializeField] protected float _fireDelay;
    [Range(.5f, 5f)]
    [SerializeField] protected float _reloadTime;
    [Range(1f, 10f)]
    [SerializeField] protected float _rotationSpeed;

    [Space]

    [Header("Field Of View")]
    [SerializeField] protected LayerMask _mask;

    [SerializeField] private Transform _rotatingPart;

    [Space]

    [Header("Debug")]
    [SerializeField] private Color _fireAreaColor = Color.red;

    protected TurretAim _aim;
    protected IDamagable _currentTarget;
    protected Quaternion _targetRot;

    protected float _reloadTimer = 0f;
    protected float _fireTimer = 0f;

    private void Awake()
    {
        _aim = GetComponent<TurretAim>();
    }

    private void Update()
    {
        if (_currentTarget != null)
        {
            if (Vector3.Distance(_currentTarget.Transform.position, transform.position) > _aim.AimDistance)
            {
                _currentTarget = null;
                return;
            }

            if (_aim.IsAimed)
            {
                Fire();
            }
            _aim.SetIdle(false);
            _aim.SetAim(_currentTarget.Transform.position);
        }
        else
        {
            _aim.SetIdle(true);
            _currentTarget = FindTarget();
        }


        if (_reloadTimer > 0f)
            _reloadTimer -= Time.deltaTime;

        if (_fireTimer > 0f)
            _fireTimer -= Time.deltaTime;
    }


    protected abstract void Fire();

    protected virtual bool CanFire()
    {
        return _currentTarget != null && _aim.IsAimed && _reloadTimer <= 0f && _fireTimer <= 0f;
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
