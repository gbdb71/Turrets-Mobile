using UnityEngine;
using ToolBox.Pools;

[RequireComponent(typeof(TrailRenderer))]
public abstract class BaseProjectile : MonoBehaviour
{
    protected TrailRenderer _trailRenderer;

    protected float _gravity = 9.81f;
    protected float _damage = 10f;
    protected Vector3 _launchPoint, _launchVelocity;

    protected virtual void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public virtual void Initialize(Vector3 launchPoint, Vector3 launchVelocity, float damage, float physicsDrop)
    {
        this._launchPoint = launchPoint;
        this._launchVelocity = launchVelocity;
        this._damage = damage;
        this._gravity = physicsDrop;

        _trailRenderer.Clear();
    }

    protected abstract void Move();

    private void Update()
    {
        Move();
    }


    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision);
        gameObject.Release();
    }

    protected virtual void Damage(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy damagable))
        {
            damagable.ApplyDamage(_damage);
        }
    }
}
