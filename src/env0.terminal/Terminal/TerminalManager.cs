using env0.terminal.Boot;
using env0.terminal.Login;
using env0.terminal.Terminal.Commands;

namespace env0.terminal.Terminal
{
    public class TerminalManager
    {
        private TerminalStateManager stateManager;
        private BootSequenceHandler bootSequenceHandler;
        private LoginHandler loginHandler;

        public TerminalManager(TerminalStateManager stateManager, BootSequenceHandler bootSequenceHandler, LoginHandler loginHandler)
        {
            this.stateManager = stateManager;
            this.bootSequenceHandler = bootSequenceHandler;
            this.loginHandler = loginHandler;
        }

        public void Start()
        {
            bootSequenceHandler.RunBootSequence();
            stateManager.SetState(TerminalState.Login);
        }

        public void StartLogin()
        {
            loginHandler.RunLogin();
            stateManager.SetState(TerminalState.Shell);
        }

        public string GetWelcomeMessage()
        {
            return "env0.terminal ready.";
        }
                public string GetPrompt()
        {
            // Pulls username from loginHandler if available
            var username = loginHandler?.Username ?? "user";
            return $"{username}@sbc:~$ ";
        }

        public string ProcessInput(string input)
        {
            var parts = input.Split(' ');
            var command = parts[0].ToLower();
            var args = parts.Skip(1).ToArray(); // Arguments after the command

            CommandHandler handler = command switch
            {
                "ls" => new LsCommand(),
                "cd" => new CdCommand(),
                "nmap" => new NmapCommand(),
                "ping" => new PingCommand(),
                _ => null
            };

            if (handler != null)
            {
                return handler.Execute(args);
            }

            return $"bash: {command}: command not found";
        }

    }
}
