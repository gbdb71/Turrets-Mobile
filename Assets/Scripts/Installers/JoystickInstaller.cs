using UnityEngine;
using Zenject;

public class JoystickInstaller : MonoInstaller
{
    [SerializeField] private Joystick _joystick;
    public override void InstallBindings()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            canvas = Container.InstantiateComponentOnNewGameObject<Canvas>();
        }

        Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
        Container.Bind<Joystick>().FromComponentInNewPrefab(_joystick).UnderTransform(canvas.transform).AsSingle();
    }
}
