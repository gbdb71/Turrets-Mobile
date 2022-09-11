using UnityEngine;
using Zenject;

public class MapInstaller : MonoInstaller
{
    [SerializeField] private Map _map;

    public override void InstallBindings()
    {
        Container.Bind<Map>().FromInstance(_map).AsSingle();
    }
}