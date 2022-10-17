using UnityEngine;
using Zenject;

public class AttackDroneAbillity : BaseAbillity
{
    [Inject] private Player _player;
    [Inject] private DiContainer _container;

    public override void Activate()
    {
        AttackDrone attackDrone = Resources.Load<AttackDrone>("GameData/Characters/AttackDrone");

        if (attackDrone != null)
        {
            _container.InstantiatePrefab(attackDrone, _player.transform.position, Quaternion.identity, null);

            Clear();
        }
    }
}
