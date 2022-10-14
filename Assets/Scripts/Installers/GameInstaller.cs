using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameLogic _game;

    public override void InstallBindings()
    {
        Container.Bind<GameLogic>().FromInstance(_game).AsSingle().NonLazy();
    }
}