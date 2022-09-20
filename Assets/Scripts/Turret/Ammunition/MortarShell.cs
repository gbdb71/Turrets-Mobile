using UnityEngine;

public class MortarShell : BaseProjectile
{
    [SerializeField, Range(1f, 2f)] private float _damageDrop = 2f;

    private float _blastRadius;

    public void Initialize(Vector3 launchPoint, Vector3 launchVelocity, float damage, float blastRadius)
    {
        base.Initialize(launchPoint, launchVelocity, damage, 9.81f);

        this._blastRadius = blastRadius;
    }

    protected override void Damage(Collision collision)
    {
        TargetPoint.FillBuffer(collision.transform.position, _blastRadius);

        for (int i = 0; i < TargetPoint.BufferedCount; i++)
        {
            Enemy enemy = TargetPoint.GetBuffered(i);
            float distance = Vector3.Distance(collision.transform.position, enemy.transform.position);

            float percent = distance / _blastRadius;
            float minDamage = (_damage / 3);
            float damage = (_damageDrop - percent) * (_damage - minDamage) + minDamage;

            enemy.ApplyDamage(damage);
        }
    }

    protected float _age;

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