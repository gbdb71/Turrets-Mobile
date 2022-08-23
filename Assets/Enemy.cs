using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public Transform Transform  => transform;

    public void Damage(float damage)
    {
        
    }
}
