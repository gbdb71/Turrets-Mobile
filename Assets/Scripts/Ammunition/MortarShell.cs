using UnityEngine;

public class MortarShell : BaseProjectile
{
    [SerializeField] private LayerMask _enemyMask;

    private float _blastRadius;

    public override void Initialize(Vector3 launchPoint, Vector3 launchVelocity,
        float blastRadius, float damage)
    {
        base.Initialize(launchPoint, launchVelocity, damage, 9.81f);

        this._blastRadius = blastRadius;
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