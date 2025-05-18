using System;
using env0.terminal.Terminal; // Add this using for your engine namespace

namespace env0.terminal.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var terminal = new TerminalManager();
            string message = terminal.GetWelcomeMessage();
            Console.WriteLine(message);
        }
    }
}
