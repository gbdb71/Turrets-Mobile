using UnityEngine;
using Zenject;

public class JoystickInstaller : MonoInstaller
{
    [SerializeField]
    private Joystick _joystick;

    public override void InstallBindings()
    {
       Container.Bind<Joystick>().FromInstance(_joystick).AsSingle();
    }
}
