using Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerIntaller : MonoInstaller
{
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private Player _player;

    public override void InstallBindings()
    {
        PlayerSpawn spawn = FindObjectOfType<PlayerSpawn>();
        Vector3 spawnPoint = spawn != null ? spawn.transform.position: Vector3.zero;

        Player player = Container.InstantiatePrefab(_player, spawnPoint, Quaternion.identity, null).GetComponent<Player>();

        CinemachineVirtualCamera cam = Instantiate(_playerCamera);
        cam.transform.position = spawnPoint;
        cam.Follow = player.transform;

        Container.Bind<CinemachineVirtualCamera>().FromInstance(cam).AsSingle();
        Container.Bind<Player>().FromInstance(player).AsSingle().NonLazy();
    }
}
