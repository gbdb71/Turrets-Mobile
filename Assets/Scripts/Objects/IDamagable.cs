using UnityEngine;

public interface IDamagable
{
    public void ApplyDamage(float damage);

    public Transform Transform { get; }
}