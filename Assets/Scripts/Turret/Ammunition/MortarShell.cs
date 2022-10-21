using UnityEngine;
using ToolBox.Pools;

public class MortarShell : BaseProjectile
{
    [SerializeField, Range(1f, 2f)] private float _damageDrop = 2f;
    [SerializeField] private GameObject _mortarDamageParticle;

    protected float _age;
    private float _blastRadius;

    public override void Initialize(Vector3 launchPoint, Vector3 launchVelocity, float damage, float blastRadius)
    {
        base.Initialize(launchPoint, launchVelocity, damage, 9.81f);
        this._blastRadius = blastRadius;
        this._age = 0f;
    }

    protected override void Damage(Collision collision)
    {

        Vector3 collisionPoint = collision.GetContact(0).point;
        _mortarDamageParticle.Reuse(collisionPoint, Quaternion.identity);

        TargetPoint.FillBuffer(collisionPoint, _blastRadius);
        for (int i = 0; i < TargetPoint.BufferedCount; i++)
        {
            Enemy enemy = TargetPoint.GetBuffered(i);
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(collisionPoint, enemy.transform.position);

            float percent = distance / _blastRadius;
            float minDamage = (_damage / 3);
            float damage = (_damageDrop - percent) * (_damage - minDamage) + minDamage;

            enemy.ApplyDamage(damage);
        }
    }
    protected override void Move()
    {
        _age += Time.deltaTime;
        Vector3 p = _launchPoint + _launchVelocity * _age;
        p.y -= 0.5f * _gravity * _age * _age;
        transform.localPosition = p;

        Vector3 d = _launchVelocity;
        d.y -= _gravity * _age;
        transform.localRotation = Quaternion.LookRotation(d);
    }
}