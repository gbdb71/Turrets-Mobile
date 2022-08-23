using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(1, 100f)]
    [SerializeField] private float _damage = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(_damage);
        }

        Destroy(gameObject);
    }
}
