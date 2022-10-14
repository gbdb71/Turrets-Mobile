using UnityEngine;
using Zenject;

public class AttackDroneAbillity : BaseAbillity
{
    [SerializeField] private GameObject _prefab;

    [Inject] private Player _player;
    [Inject] private DiContainer _container;
    protected override void Activate()
    {
        base.Activate();

        _container.InstantiatePrefab(_prefab, _player.transform.position, Quaternion.identity, null);
        //_system.RemoveAbillity(this);
    }

    public override void Clear()
    {
    }
}
