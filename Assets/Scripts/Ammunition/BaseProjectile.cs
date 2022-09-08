using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    protected float _gravity = 9.81f;

    protected float _damage = 10f;
    protected Vector3 _launchPoint, _launchVelocity;

    public virtual void Initialize(Vector3 launchPoint, Vector3 launchVelocity, float damage, float physicsDrop)
    {
        this._launchPoint = launchPoint;
        this._launchVelocity = launchVelocity;
        this._damage = damage;
        this._gravity = physicsDrop;
    }

    private float age;
    private void Update()
    {
        age += Time.deltaTime;
        Vector3 p = _launchPoint + _launchVelocity * age;
        p.y -= 0.5f * _gravity * age * age;
        transform.localPosition = p;

        Vector3 d = _launchVelocity;
        d.y -= _gravity * age;
        transform.localRotation = Quaternion.LookRotation(d);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision);
        Destroy(gameObject);
    }

    protected virtual void Damage(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy damagable))
        {
            damagable.ApplyDamage(_damage);
        }
    }
}
