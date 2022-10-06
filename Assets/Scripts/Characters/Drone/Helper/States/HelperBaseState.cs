using Assets.Scripts.StateMachine;

namespace Characters.Drone.Helper
{
    public class HelperBaseState : State
    {
        protected new HelperStateMachine _stateMachine;

        public HelperBaseState(HelperStateMachine stateMachine) : base(stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void Update()
        {

        }
    }
}
