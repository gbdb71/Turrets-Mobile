using UnityEngine;
using Zenject;

public class JoystickInstaller : MonoInstaller
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Canvas _canvas;
    public override void InstallBindings()
    {
        Container.Bind<Joystick>().FromComponentInNewPrefab(_joystick).UnderTransform(_canvas.transform).AsSingle();
    }
}
