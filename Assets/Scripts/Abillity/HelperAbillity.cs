using UnityEngine;
using Zenject;

public class HelperAbillity : BaseAbillity
{
    [SerializeField] private GameObject _prefab;

    [Inject] private Game _game;
    [Inject] private DiContainer _container;
    protected override void Activate()
    {
        base.Activate();

        _container.InstantiatePrefab(_prefab, _game.Headquarters.DronePoint.position, Quaternion.identity, null);
    }

    public override void Clear()
    {
    }
}

