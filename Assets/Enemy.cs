using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public Transform Transform  => transform;

    public void ApplyDamage(float damage)
    {
        Debug.Log("Taked damage: " + damage);
    }
}
