namespace Assets.Scripts.StateMachine
{
    public abstract class State
    {
        protected StateMachine _stateMachine;

        public State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
