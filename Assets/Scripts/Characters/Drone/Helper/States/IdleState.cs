namespace Characters.Drone.Helper
{
    public class IdleState : HelperBaseState
    {
        public IdleState(HelperStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Update()
        {
            if (!_stateMachine.Owner.InventoyEmpty && BaseTurret.Turrets.Count > 0)
            {
                BaseTurret turret = _stateMachine.Owner.GetTargetTurret();

                if (turret != null)
                {
                    _stateMachine.ChargeTurretState.SetTarget(turret);
                    _stateMachine.ChangeState(_stateMachine.ChargeTurretState);
                    return;
                }
            }
            else if (!_stateMachine.Owner.InventoryFull && Factory.Factories.Count > 0)
            {
                Ammunition target = _stateMachine.Owner.GetTargetAmmunition();

                if (target != null)
                {
                    _stateMachine.TakeAmmoState.SetTarget(target);
                    _stateMachine.ChangeState(_stateMachine.TakeAmmoState);
                    return;
                }
            }

            _stateMachine.Owner.Agent.SetDestination(_stateMachine.Owner.Game.Headquarters.DronePoint.position);
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }
    }
}
