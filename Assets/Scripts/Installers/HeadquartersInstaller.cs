using Zenject;

public class HeadquartersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Headquarters headquarters = FindObjectOfType<Headquarters>();

        Container.Bind<Headquarters>().FromInstance(headquarters).AsSingle().NonLazy();
    }
}
