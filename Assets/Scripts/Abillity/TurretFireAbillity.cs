using UnityEngine;
using Zenject;

public class TurretFireAbillity : BaseAbillity<FireAbillitySettings>
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _damagePercents = new Vector2(1, 10);
    [Inject] private Player _player;

    public override void Activate()
    {
        base.Activate();

        if (_player.Inventory.NearTurret != null)
        {
            _player.Inventory.NearTurret.DecreaseFireDelay(Random.Range(_damagePercents.x, _damagePercents.y));
            _player.Inventory.NearTurret.PlayUpgradeParticle();

            Clear();
        }
    }

    public override bool CanActivate() => _player.Inventory.NearTurret != null;
}

[CreateAssetMenu(fileName = "FireAbillity", menuName = "TowerDefense/Abillities/Turret Fire")]
public class FireAbillitySettings : BaseAbillityConfig
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _firePercents = new Vector2(1, 10);

    public float GetFirePercents() => Random.Range(_firePercents.x, _firePercents.y);
}
