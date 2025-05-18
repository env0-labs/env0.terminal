using System;
using env0.terminal;

namespace env0.terminal.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new TerminalEngine("src/env0.terminal/Boot/BootConfig.json");

            // Boot Sequence
            engine.RunBoot();

            // Login Sequence
            if (engine.GetState() == Terminal.TerminalState.Login)
            {
                engine.RunLogin();
            }

            // Shell Loop
            while (engine.GetState() == Terminal.TerminalState.Shell)
            {
                Console.Write(engine.GetPrompt());
                var input = Console.ReadLine();
                var output = engine.ProcessInput(input);
                Console.WriteLine(output);
            }

            Console.WriteLine("Session ended. (Not implemented: Shell exit handling)");
        }
    }
}
