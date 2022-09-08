using UnityEngine;
using UnityEngine.VFX;

public class IceTurret : BaseTurret
{
    [Label("Ice Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 180)] protected float _damageAngle = 20f;
    [SerializeField, Range(.001f, 1f)] protected float _attackRate = .1f;
    [SerializeField, NotNull] protected VisualEffect _throwEffect;

    public bool IsFire { get; private set; }

    protected void Start()
    {
        _throwEffect.SetFloat("Angle", _damageAngle);
    }

    float fireTime, attack = 0f;
    protected override void Fire()
    {
        if (!IsFire)
        {
            IsFire = true;
            _throwEffect.Play();
        }

        fireTime += Time.deltaTime;
        attack += Time.deltaTime;

        if (attack >= _attackRate)
        {
            attack = 0f;
            DamageArea();
        }

        if (fireTime >= 1f)
        {
            fireTime = 0f;
            _chargedAmmo -= 1;
        }
    }

    private void DamageArea()
    {
        if (TargetPoint.FillBuffer(_shootPivot.position, _aim.AimDistance))
        {
            for (int i = 0; i < TargetPoint.BufferedCount; i++)
            {
                Enemy enemy = TargetPoint.GetBuffered(i);

                Vector3 targetDirection = enemy.transform.position - _aim.ArcRoot.transform.position;
                float angleBetween = Vector3.Angle(_aim.ArcRoot.transform.forward, targetDirection);

                if (angleBetween <= _damageAngle)
                {
                    enemy.ApplyDamage(_damage);
                }
            }
        }
    }

    protected override void StopFire()
    {
        if (IsFire)
        {
            IsFire = false;
            _throwEffect.Stop();
        }
    }

#if UNITY_EDITOR
    // This should probably go in an Editor script, but dealing with Editor scripts
    // is a pain in the butt so I'd rather not.
    private void OnDrawGizmosSelected()
    {
        if (_aim == null)
        {
            _aim = GetComponent<TurretAim>();
        }

        if (!_aim.DrawDebugArcs)
            return;

        if (_aim.TurretBase != null)
        {
            float kArcSize = _aim.AimDistance;

            Color colorDamage = new Color(1f, .5f, .5f, .1f);

            UnityEditor.Handles.color = colorDamage;

            UnityEditor.Handles.DrawSolidArc(
                _aim.ArcRoot.position, _aim.TurretBase.up,
                 _aim.ArcRoot.transform.forward, _damageAngle,
                kArcSize);
            UnityEditor.Handles.DrawSolidArc(
                _aim.ArcRoot.position, _aim.TurretBase.up,
                 _aim.ArcRoot.transform.forward, -_damageAngle,
                kArcSize);
        }
    }
#endif
}
