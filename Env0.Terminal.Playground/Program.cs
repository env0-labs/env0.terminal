using System;
using Env0.Terminal;
using Env0.Terminal.API_DTOs;

namespace Env0.Terminal.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            Console.WriteLine("env0.terminal playground (type 'exit' to quit)");
            Console.WriteLine("----------------------------------------------");

            // Start by requesting the first render state (boot phase)
            TerminalRenderState state = api.Execute(""); // First call

            while (true)
            {
                switch (state.Phase)
                {
                    case TerminalPhase.Booting:
                        if (state.BootSequenceLines != null)
                        {
                            foreach (var line in state.BootSequenceLines)
                                Console.WriteLine(line);
                        }
                        // Move to login phase
                        state = api.Execute("");
                        continue;

                    case TerminalPhase.Login:
                        while (state.Phase == TerminalPhase.Login)
                        {
                            if (!string.IsNullOrWhiteSpace(state.Output))
                                Console.WriteLine(state.Output);

                            if (state.IsLoginPrompt)
                            {
                                Console.Write(state.Prompt ?? "Username: ");
                                var username = Console.ReadLine();
                                if (username == null) continue;
                                state = api.Execute(username);
                            }
                            else if (state.IsPasswordPrompt)
                            {
                                Console.Write(state.Prompt ?? "Password: ");
                                var password = ReadPassword();
                                if (password == null) continue;
                                state = api.Execute(password);
                            }
                            else
                            {
                                break; // Unexpected state, return to outer loop
                            }
                        }
                        continue;

                    case TerminalPhase.Terminal:
                    default:
                        if (!string.IsNullOrWhiteSpace(state.Output))
                        {
                            Console.WriteLine(state.Output);
                            state.Output = null;
                        }

                        while (true)
                        {
                            Console.Write(state.Prompt);
                            var input = Console.ReadLine();
                            if (input == null) continue;

                            // === Improved exit handling with random, stylized confirmation ===
                            if (input.Trim().ToLower() == "exit")
                            {
                                state = api.Execute("exit");
                                if (state.SessionStackDepth == 0 && state.Phase == TerminalPhase.Terminal)
                                {
                                    var confirmations = new[]
                                    {
                                        "Are you sure you want to exit? (y/n): ",
                                        "Exit? The abyss stares back. (y/n): ",
                                        "Last chance: close all sessions? (y/n): ",
                                        "You do realize this will end your only connection to this universe, right? (y/n): ",
                                        "Confirm session termination [Y/n]? "
                                    };
                                    var rand = new Random();
                                    Console.Write(confirmations[rand.Next(confirmations.Length)]);
                                    var answer = Console.ReadLine();
                                    if (!string.IsNullOrEmpty(answer) && answer.Trim().ToLower() == "y")
                                    {
                                        Console.WriteLine("Session ended. Bye!");
                                        return;
                                    }
                                    // Otherwise, stay in Playground—re-display prompt
                                    continue;
                                }
                                // Otherwise, show output (like "Connection to ... closed.") and continue at the new prompt/session
                                if (!string.IsNullOrWhiteSpace(state.Output))
                                {
                                    Console.WriteLine(state.Output);
                                    state.Output = null;
                                }
                                continue;
                            }

                            state = api.Execute(input);

                            // If command moves us into login phase (e.g., SSH), break to outer loop
                            if (state.Phase == TerminalPhase.Login)
                                break;

                            if (!string.IsNullOrWhiteSpace(state.Output))
                            {
                                Console.WriteLine(state.Output);
                                state.Output = null;
                            }

                            if (state.IsError && !string.IsNullOrWhiteSpace(state.ErrorMessage))
                                Console.WriteLine($"[ERROR] {state.ErrorMessage}");

                            if (state.ShowMOTD && !string.IsNullOrWhiteSpace(state.MOTD))
                                Console.WriteLine($"[MOTD] {state.MOTD}");
                        }
                        continue;
                }
            }
        }

        /// <summary>
        /// Reads a password from the console without echoing input.
        /// </summary>
        static string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                        password = password.Substring(0, password.Length - 1);
                    continue;
                }
                if (char.IsControl(key.KeyChar))
                    continue;
                password += key.KeyChar;
            }
            Console.WriteLine();
            return password;
        }
    }
}
