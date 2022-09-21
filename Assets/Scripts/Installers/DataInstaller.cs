using UnityEngine;
using Zenject;

[RequireComponent(typeof(Data))]
public class DataInstaller : MonoInstaller
{
    [SerializeField] private Data _data;

    public override void InstallBindings()
    {
        Container.Bind<Data>().FromInstance(_data).AsSingle().NonLazy();
    }
}