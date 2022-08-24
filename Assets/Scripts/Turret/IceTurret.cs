using UnityEngine;
using UnityEngine.VFX;

public class IceTurret : BaseTurret<Object>
{
    [Header("Throw Attack")]
    [Range(1, 180)]
    [SerializeField] protected float _damageAngle = 20f;
    [SerializeField] protected float _attackRate = .1f;
    [SerializeField] protected VisualEffect _throwEffect;


    protected void Start()
    {
        _throwEffect.SetFloat("Angle", _damageAngle);
    }

    float fireTime = 0f;
    protected override void Fire()
    {
        base.Fire();

        _throwEffect.Play();

        fireTime += Time.deltaTime;

        if (fireTime >= 1f)
        {
            fireTime = 0f;
            _chargedAmmo -= 1;
        }
    }

    protected override void StopFire()
    {
        _throwEffect.Stop();
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
                transform.forward, _damageAngle,
                kArcSize);
            UnityEditor.Handles.DrawSolidArc(
                _aim.ArcRoot.position, _aim.TurretBase.up,
                transform.forward, -_damageAngle,
                kArcSize);
        }
    }
#endif
}
