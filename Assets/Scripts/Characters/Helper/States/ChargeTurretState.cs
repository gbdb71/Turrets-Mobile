
using Assets.Scripts.StateMachine;

public class ChargeTurretState : HelperBaseState
{
    private BaseTurret _targetTurret;

    public ChargeTurretState(HelperStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
       
    }
    public override void Update()
    {
        if(_stateMachine.Owner.InventoyEmpty)
        {
            _stateMachine.ChangeState(_stateMachine.TakeAmmoState);
            return;
        }

        if(_targetTurret != null)
        {
            if (!_targetTurret.CanCharge)
                _targetTurret = null;
        }

        if(_targetTurret == null)
        {
            _targetTurret = GetTargetTurret();

            if( _targetTurret == null)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }
        }

        _stateMachine.Owner.Agent.SetDestination(_targetTurret.transform.position);
    }
    public override void Exit()
    {
       
    }

    private BaseTurret GetTargetTurret()
    {
        for (int i = 0; i < BaseTurret.Turrets.Count; i++)
        {
            BaseTurret turret = BaseTurret.Turrets[i];

            if(turret.gameObject.activeSelf && turret.CanCharge)
            {
                return turret;
            }
        }

        return null;
    }
}

