using System.Linq;

public class IdleState : HelperBaseState
{
    public IdleState(HelperStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Update()
    {
        if (!_stateMachine.Owner.InventoyEmpty && BaseTurret.Turrets.Count > 0)
        {
            _stateMachine.ChangeState(_stateMachine.ChargeTurretState);
        }
        else if (!_stateMachine.Owner.InventoryFull && Factory.Factories.Count > 0)
        {
            Factory target = GetTargetFactory();

            if (target != null)
            {
                _stateMachine.TakeAmmoState.SetTarget(target);
                _stateMachine.ChangeState(_stateMachine.TakeAmmoState);
            }
        }

        _stateMachine.Owner.Agent.SetDestination(_stateMachine.Owner.Game.Headquarters.DronePoint.position);
    }


    public Factory GetTargetFactory()
    {
        for (int i = 0; i < Factory.Factories.Count; i++)
        {
            Factory factory = Factory.Factories[i];

            if (factory.gameObject.activeSelf &&
                factory.Type == FactoryType.Ammunition &&
                factory.Plates.Count > 0 &&
                factory.Plates.Any(x => x.CanPlace() == false))
            {
                return factory;
            }
        }


        return null;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }
}

