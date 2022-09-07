using UnityEngine;

public class AutomaticTurret : BaseTurret
{
    [Label("Bullet Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(2, 40)] protected float _shootForce;

    protected override void Fire()
    {
        base.Fire();

        Rigidbody projectile = Instantiate(_projectilePrefab, _shootPivot.transform.position, _shootPivot.transform.rotation);
        projectile.AddForce(projectile.transform.forward * _shootForce, ForceMode.Impulse);
    }
}
