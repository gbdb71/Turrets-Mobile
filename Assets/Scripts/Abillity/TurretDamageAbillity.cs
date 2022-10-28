
using UnityEngine;

public class TurretDamageAbillity : BaseAbillity<DamageAbillitySettings>
{
    public override void Activate()
    {
        base.Activate();

        if (_player.Inventory.NearPlace != null)
        {
            _player.Inventory.NearPlace.PlacedTurret.AddDamageValue(_config.GetDamagePercents());
            _player.Inventory.NearPlace.PlacedTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate() => _player.Inventory.NearPlace != null;
    public override Vector3 PopupPosition => _player.Inventory.NearPlace.transform.position + new Vector3(0, 2.2f, 0);
}
