using Zenject;

public class TurretDamageAbillity : BaseAbillity<DamageAbillitySettings>
{
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
