namespace env0.terminal.Terminal
{
    public enum TerminalState
    {
        Boot,
        Login,
        Shell
    }

    public class TerminalStateManager
    {
        public TerminalState CurrentState { get; private set; }

        public TerminalStateManager()
        {
            CurrentState = TerminalState.Boot; // Default to Boot at start
        }

        public void SetState(TerminalState newState)
        {
            CurrentState = newState;
        }
    }
}
