using UnityEngine;

[CreateAssetMenu(fileName = "AttackDrone", menuName = "TowerDefense/Abillities/Attack Drone")]
public class DroneAbillitySettings : BaseAbillityConfig
{
    [AssetPreview, SerializeField, NotNull] private AttackDrone _dronePrefab;

    public AttackDrone DronePrefab => _dronePrefab;
}
