using System.Collections.Generic;

public class TakeAmmoState : HelperBaseState
{
    private Factory _targetFactory;
    private Ammunition _target;

    public TakeAmmoState(HelperStateMachine stateMachine) : base(stateMachine)
    {
    }


    public void SetTarget(Factory target)
    {
        _targetFactory = target;
    }


    public override void Enter()
    {

    }
    public override void Update()
    {
        if(_stateMachine.Owner.InventoryFull)
        {
            _stateMachine.ChangeState(_stateMachine.ChargeTurretState);
            return;
        }

        if (_targetFactory == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
        else
        {
            if(_target != null)
            {
                if (!_target.enabled)
                {
                    _target = null;
                    Helper.TargetAmmunitions.Remove(_target);
                }
            }

            if(_target == null)
            {
                _target = _stateMachine.Owner.GetTargetAmmunition(_targetFactory);

                if(_target == null)
                {
                    _targetFactory = null;
                    return;
                }

                Helper.TargetAmmunitions.Add(_target);
            }

            _stateMachine.Owner.Agent.SetDestination(_target.transform.position);
        }
    }
    public override void Exit()
    {
        Helper.TargetAmmunitions.Remove(_target);
    }

}

