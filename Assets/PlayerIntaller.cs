using UnityEngine;
using Zenject;

public class PlayerIntaller : MonoInstaller
{
    [SerializeField] private Player _player;

    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(_player).AsSingle().NonLazy();
    }
}
