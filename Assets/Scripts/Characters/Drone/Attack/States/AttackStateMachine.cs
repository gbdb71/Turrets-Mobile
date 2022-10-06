using Assets.Scripts.StateMachine;

namespace Characters.Drone.Attack
{
    public class AttackStateMachine : StateMachine
    {
        public readonly AttackDrone Owner;
        public IdleState IdleState { get; }


        public AttackStateMachine(AttackDrone owner) : base()
        {
            Owner = owner;


            IdleState = (IdleState)_currentState;
        }


        protected override State GetInitialState()
        {
            return new IdleState(this);
        }
    }
}