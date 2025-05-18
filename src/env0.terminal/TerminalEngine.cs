using env0.terminal.Terminal;
using env0.terminal.Boot;
using env0.terminal.Login;

namespace env0.terminal
{
    public class TerminalEngine
    {
        private TerminalStateManager stateManager;
        private BootSequenceHandler bootSequenceHandler;
        private LoginHandler loginHandler;
        private TerminalManager terminalManager;

        public TerminalEngine(
            string bootConfigPath
            // In future: paths for users, filesystems, etc.
        )
        {
            // Initialize all modules with basic dependencies
            stateManager = new TerminalStateManager();
            bootSequenceHandler = new BootSequenceHandler(bootConfigPath);
            loginHandler = new LoginHandler();
            terminalManager = new TerminalManager(stateManager, bootSequenceHandler, loginHandler);
        }

        public TerminalState GetState() => stateManager.CurrentState;

        // Forwards to boot logic
        public void RunBoot()
        {
            terminalManager.Start();
        }

        // Forwards to login logic
        public void RunLogin()
        {
            terminalManager.StartLogin();
        }

        // Returns shell prompt string (after login)
        public string GetPrompt()
        {
            return terminalManager.GetPrompt();
        }

        // Handles shell input (for now: just echo)
        public string ProcessInput(string input)
        {
            return terminalManager.ProcessInput(input);
        }

        // Expose username (after login)
        public string GetUsername()
        {
            return loginHandler.Username;
        }
    }
}
