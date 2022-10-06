using Assets.Scripts.StateMachine;

namespace Characters.Drone.Attack
{
    public class AttackBaseState : State
    {
        protected new AttackStateMachine _stateMachine;

        public AttackBaseState(AttackStateMachine stateMachine) : base(stateMachine)
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
