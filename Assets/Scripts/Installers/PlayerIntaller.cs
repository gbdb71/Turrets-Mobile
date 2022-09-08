using Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerIntaller : MonoInstaller
{
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private Player _player;


    public override void InstallBindings()
    {
        Vector3 spawnPoint = _spawnPoint != null ? Vector3.zero : _playerCamera.transform.position;
        Player player = Container.InstantiatePrefab(_player, spawnPoint, Quaternion.identity, null).GetComponent<Player>();

        CinemachineVirtualCamera cam = Instantiate(_playerCamera);
        cam.transform.position = spawnPoint;
        cam.Follow = player.transform;

        Container.Bind<CinemachineVirtualCamera>().FromInstance(cam).AsSingle(); 
        Container.Bind<Player>().FromInstance(player).AsSingle().NonLazy();

        player.gameObject.SetActive(false);
    }
}
