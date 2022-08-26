using UnityEngine;

public class AutomaticTurret : BaseTurret
{
    [Header("Bullet Settings")]
    [Range(2, 40)]
    [SerializeField] protected float _shootForce;

    protected override void Fire()
    {
        base.Fire();

        Rigidbody projectile = Instantiate(_projectilePrefab, _shootPivot.transform.position, _shootPivot.transform.rotation);
        projectile.AddForce(projectile.transform.forward * _shootForce, ForceMode.Impulse);
    }
}
