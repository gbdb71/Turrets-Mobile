namespace Assets.Scripts.StateMachine
{
    public abstract class StateMachine
    {
        protected State _currentState;

        public StateMachine()
        {
            _currentState = GetInitialState();
        }

        protected abstract State GetInitialState();

        public void Update()
        {
            _currentState.Update();
        }

        public void ChangeState(State state)
        {
            if (state == _currentState)
                return;

            _currentState.Exit();

            _currentState = state;
            _currentState.Enter();
        }
    }
}
