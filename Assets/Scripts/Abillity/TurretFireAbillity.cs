using UnityEngine;

public class TurretFireAbillity : BaseAbillity<FireAbillitySettings>
{
    public override void Activate()
    {
        base.Activate();

        if (_player.Inventory.NearPlace != null && _player.Inventory.NearPlace.PlacedTurret != null)
        {
            _player.Inventory.NearPlace.PlacedTurret.DecreaseFireDelay(_config.GetFirePercents());
            _player.Inventory.NearPlace.PlacedTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate() => _player.Inventory.NearPlace != null;
    public override Vector3 PopupPosition => _player.Inventory.NearPlace.transform.position + new Vector3(0, 2.2f, 0);
}
