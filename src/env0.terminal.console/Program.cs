using System;
using env0.terminal.Terminal;

namespace env0.terminal.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var stateManager = new TerminalStateManager();
            var terminal = new TerminalManager(stateManager);

            string message = terminal.GetWelcomeMessage();
            Console.WriteLine(message);

            // For now, just display current state as a demo:
            Console.WriteLine($"Current state: {stateManager.CurrentState}");
        }
    }
}
