using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameLogic>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}