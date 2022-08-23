using UnityEngine;

public class AutomaticTurret : BaseTurret
{
    [Header("Bullet Settings")]
    [Range(2, 40)]
    [SerializeField] protected float _shootForce;

    protected override void Fire()
    {
        Rigidbody bullet = Instantiate(_ammunitionPrefab, _shootPivot.transform.position, _shootPivot.transform.rotation);

        bullet.AddForce(bullet.transform.forward * _shootForce, ForceMode.Impulse);

        _fireTimer = _fireDelay;
        _chargedAmmo -= 1;
    }
}
