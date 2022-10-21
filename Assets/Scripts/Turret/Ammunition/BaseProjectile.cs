using UnityEngine;
using ToolBox.Pools;

[RequireComponent(typeof(LineRenderer))]
public abstract class BaseProjectile : MonoBehaviour
{
    protected LineRenderer _lineRenderer;

    protected float _gravity = 9.81f;
    protected float _damage = 10f;
    protected Vector3 _launchPoint, _launchVelocity;

    protected virtual void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public virtual void Initialize(Vector3 launchPoint, Vector3 launchVelocity, float damage, float physicsDrop)
    {
        this._launchPoint = launchPoint;
        this._launchVelocity = launchVelocity;
        this._damage = damage;
        this._gravity = physicsDrop;
    }

    protected abstract void Move();

    private void Update()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision);

        _lineRenderer.positionCount = 0;
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
