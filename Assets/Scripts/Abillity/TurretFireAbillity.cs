using Zenject;

public class TurretFireAbillity : BaseAbillity<FireAbillitySettings>
{
    public override void Activate()
    {
        base.Activate();

        if (_player.Inventory.NearTurret != null)
        {
            _player.Inventory.NearTurret.DecreaseFireDelay(_config.GetFirePercents());
            _player.Inventory.NearTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate() => _player.Inventory.NearTurret != null;
}
