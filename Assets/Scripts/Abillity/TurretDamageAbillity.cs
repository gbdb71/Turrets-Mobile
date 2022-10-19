using UnityEngine;
using Zenject;

public class TurretDamageAbillity : BaseAbillity<DamageAbillitySettings>
{
    [Inject] private Player _player;

    public override void Activate()
    {
        base.Activate();

        if (_player.Inventory.NearTurret != null)
        {
            _player.Inventory.NearTurret.AddDamageValue(_config.GetDamagePercents());
            _player.Inventory.NearTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate() => _player.Inventory.NearTurret != null;
}


[CreateAssetMenu(fileName = "DamageAbillity", menuName = "TowerDefense/Abillities/Turret Damage")]
public class DamageAbillitySettings : BaseAbillityConfig
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _damagePercents = new Vector2(1, 10);

    public float GetDamagePercents() => Random.Range(_damagePercents.x, _damagePercents.y);
}
