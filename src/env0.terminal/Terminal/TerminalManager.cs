namespace env0.terminal.Terminal
{
    public class TerminalManager
    {
        private TerminalStateManager stateManager;

        public TerminalManager(TerminalStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public string GetWelcomeMessage()
        {
            return "env0.terminal ready.";
        }
    }
}
