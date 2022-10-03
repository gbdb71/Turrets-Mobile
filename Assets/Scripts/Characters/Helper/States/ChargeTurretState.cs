public class ChargeTurretState : HelperBaseState
{
    private BaseTurret _targetTurret;

    public ChargeTurretState(HelperStateMachine stateMachine) : base(stateMachine)
    {
    }

    public void SetTarget(BaseTurret turret)
    {
        _targetTurret = turret;
        Helper.TargetTurrets.Add(turret);
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
            {
                Helper.TargetTurrets.Remove(_targetTurret);
                _targetTurret = null;
            }
        }

        if(_targetTurret == null)
        {
            _targetTurret = _stateMachine.Owner.GetTargetTurret();

            if( _targetTurret == null)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            Helper.TargetTurrets.Add(_targetTurret);
        }

        _stateMachine.Owner.Agent.SetDestination(_targetTurret.transform.position);
    }

    public override void Exit()
    {
        Helper.TargetTurrets.Remove(_targetTurret);
        _targetTurret = null;
    }
}

