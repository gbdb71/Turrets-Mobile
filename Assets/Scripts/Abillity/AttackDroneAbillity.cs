using UnityEngine;
using Zenject;

public class AttackDroneAbillity : BaseAbillity<DroneAbillitySettings>
{
    [Inject] private Player _player;
    [Inject] private DiContainer _container;

    public override void Activate()
    {
        base.Activate();

        AttackDrone attackDrone = Resources.Load<AttackDrone>("GameData/Characters/AttackDrone");

        if (attackDrone != null)
        {
            _container.InstantiatePrefab(attackDrone, _player.transform.position, Quaternion.identity, null);

            Clear();
        }
    }
}

[CreateAssetMenu(fileName = "AttackDrone", menuName = "TowerDefense/Abillities/Attack Drone")]
public class DroneAbillitySettings : BaseAbillityConfig
{
    [AssetPreview, SerializeField, NotNull] private AttackDrone _dronePrefab;

    public AttackDrone DronePrefab => _dronePrefab;
}
