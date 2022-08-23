using UnityEngine;

public interface IDamagable
{
    public void Damage(float damage);

    public Transform Transform { get; }
}