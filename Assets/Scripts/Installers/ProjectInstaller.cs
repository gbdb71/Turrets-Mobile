using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Data>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}