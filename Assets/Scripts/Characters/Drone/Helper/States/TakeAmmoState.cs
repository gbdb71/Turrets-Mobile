namespace Characters.Drone.Helper
{
    public class TakeAmmoState : HelperBaseState
    {
        private Ammunition _target;

        public TakeAmmoState(HelperStateMachine stateMachine) : base(stateMachine)
        {
        }

        public void SetTarget(Ammunition target)
        {
            _target = target;
            HelperDrone.TargetAmmunitions.Add(_target);
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            if (_stateMachine.Owner.InventoryFull)
            {
                _stateMachine.ChangeState(_stateMachine.ChargeTurretState);
                return;
            }

            if (_target != null)
            {
                if (!_target.enabled)
                {
                    HelperDrone.TargetAmmunitions.Remove(_target);
                    _target = null;
                }
            }

            if (_target == null)
            {
                _target = _stateMachine.Owner.GetTargetAmmunition();

                if (_target == null)
                {
                    _stateMachine.ChangeState(_stateMachine.IdleState);
                    return;
                }

                HelperDrone.TargetAmmunitions.Add(_target);
            }

            _stateMachine.Owner.Agent.SetDestination(_target.transform.position);

        }
        public override void Exit()
        {
            HelperDrone.TargetAmmunitions.Remove(_target);
            _target = null;
        }

    }
}
