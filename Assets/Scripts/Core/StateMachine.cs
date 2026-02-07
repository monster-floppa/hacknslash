using System;

namespace HacknSlash.Core
{
    public class StateMachine<TState> where TState : struct
    {
        public TState CurrentState { get; private set; }
        public TState PreviousState { get; private set; }

        public event Action<TState, TState> OnStateChanged;

        public void Initialize(TState state)
        {
            CurrentState = state;
            PreviousState = state;
        }

        public void ChangeState(TState newState)
        {
            if (CurrentState.Equals(newState))
            {
                return;
            }

            PreviousState = CurrentState;
            CurrentState = newState;

            if (OnStateChanged != null)
            {
                OnStateChanged(PreviousState, CurrentState);
            }
        }
    }
}
