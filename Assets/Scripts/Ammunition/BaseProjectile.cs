using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [Range(1, 100f)]
    [SerializeField] protected float _damage = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Damage(collision);
        Destroy(gameObject);
    }

    protected virtual void Damage(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            damagable.ApplyDamage(_damage);
        }
    }
}
