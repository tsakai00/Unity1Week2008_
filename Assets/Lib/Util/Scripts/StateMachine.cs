using System.Collections;
using UnityEngine;

namespace Lib.Util
{
    public enum StateMachineCase
    {
        Enter,
        Exec,
        Exit
    };

    /// <summary>
    /// 有限ステートマシン
    /// </summary>
    public class StateMachine
    {
        public delegate void State(StateMachineCase c);

        public State Curr { get; private set; }
        public State Prev { get; private set; }
        public State Next { get; private set; }

        public StateMachine(State state)
        {
            ChangeState(state);
        }

        public void Update()
        {
            ChangeNext();
            Curr?.Invoke(StateMachineCase.Exec);
        }

        public void Exit()
        {
            Curr?.Invoke(StateMachineCase.Exit);
            Curr = Prev = Next =null;
        }

        public void ChangeState(State state)
        {
            Next = state;
        }

        private void ChangeNext()
        {
            if(Next == null) { return; }
            if(Next == Curr) { return; }

            Curr?.Invoke(StateMachineCase.Exit);
            Next?.Invoke(StateMachineCase.Enter);
            Prev = Curr;
            Curr = Next;
            Next = null;
        }
    }
}
