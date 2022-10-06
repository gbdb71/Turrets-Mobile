using Assets.Scripts.StateMachine;

namespace Characters.Drone.Helper
{
    public class HelperStateMachine : StateMachine
    {
        public readonly HelperDrone Owner;
        public IdleState IdleState { get; }
        public ChargeTurretState ChargeTurretState { get; }
        public TakeAmmoState TakeAmmoState { get; }


        public HelperStateMachine(HelperDrone owner) : base()
        {
            Owner = owner;

            ChargeTurretState = new ChargeTurretState(this);
            TakeAmmoState = new TakeAmmoState(this);
            IdleState = (IdleState)_currentState;
        }


        protected override State GetInitialState()
        {
            return new IdleState(this);
        }
    }
}