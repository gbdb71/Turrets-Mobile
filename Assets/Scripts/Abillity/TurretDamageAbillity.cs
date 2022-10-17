using UnityEngine;
using Zenject;

public class TurretDamageAbillity : BaseAbillity
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _damagePercents = new Vector2(1, 10); 
    [Inject] private Player _player;

    public override void Activate()
    {
        if(_player.Inventory.NearTurret != null)
        {
            _player.Inventory.NearTurret.AddDamageValue(Random.Range(_damagePercents.x, _damagePercents.y));
            _player.Inventory.NearTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate => _player.Inventory.NearTurret != null;
}

