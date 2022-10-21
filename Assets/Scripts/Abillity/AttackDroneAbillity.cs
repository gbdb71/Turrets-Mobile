using UnityEngine;
using Zenject;

public class AttackDroneAbillity : BaseAbillity<DroneAbillitySettings>
{
    [Inject] private DiContainer _container;

    public override void Activate()
    {
        base.Activate();

        if (_config.DronePrefab != null)
        {
            _container.InstantiatePrefab(_config.DronePrefab, _player.transform.position, Quaternion.identity, null);

            Clear();
        }
    }

    public override bool HasDelay() => false;
}
