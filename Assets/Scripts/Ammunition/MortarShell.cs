using UnityEngine;

public class MortarShell : BaseProjectile
{
    [Header("Damage")]
    [SerializeField] private LayerMask _enemyMask;

    Vector3 _launchPoint, _launchVelocity;

    public void Initialize(
        Vector3 launchPoint, Vector3 launchVelocity,
        float blastRadius, float damage)
    {
        this._launchPoint = launchPoint;
        this._launchVelocity = launchVelocity;
        this._blastRadius = blastRadius;
        this._damage = damage;
    }

    float age, _blastRadius, _damage;
    private void Update()
    {
        age += Time.deltaTime;
        Vector3 p = _launchPoint + _launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.localPosition = p;

        Vector3 d = _launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);
    }

    protected override void Damage(Collision collision)
    {
        TargetPoint.FillBuffer(transform.position, _blastRadius);
        for (int i = 0; i < TargetPoint.BufferedCount; i++)
        {
            TargetPoint.GetBuffered(i).ApplyDamage(_damage);
        }
    }
}