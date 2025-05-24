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

            TerminalRenderState state = api.Execute(""); // Should trigger boot phase

            while (true)
            {
                switch (state.Phase)
                {
                    case TerminalPhase.Booting:
                        // Print boot lines and advance phase
                        if (state.BootSequenceLines != null)
                        {
                            foreach (var line in state.BootSequenceLines)
                                Console.WriteLine(line);
                        }
                        // Move to login phase (call Execute again with no input)
                        state = api.Execute("");
                        continue;

                    case TerminalPhase.Login:
                        if (state.IsLoginPrompt)
                        {
                            Console.Write(state.Prompt ?? "Username: ");
                            var username = Console.ReadLine();
                            if (username == null) continue;
                            state = api.Execute(username);
                            continue;
                        }
                        else if (state.IsPasswordPrompt)
                        {
                            Console.Write(state.Prompt ?? "Password: ");
                            var password = ReadPassword(); // Now masks password entry
                            if (password == null) continue;
                            state = api.Execute(password);
                            continue;
                        }
                        break;

                    case TerminalPhase.Terminal:
                    default:
                        // ---- FIX: Show any output that was returned when entering terminal phase (e.g. login flavor)
                        if (!string.IsNullOrWhiteSpace(state.Output))
                            Console.WriteLine(state.Output);

                        Console.Write(state.Prompt);
                        var input = Console.ReadLine();
                        if (input == null) continue;
                        if (input.Trim().ToLower() == "exit") break;

                        state = api.Execute(input);

                        if (!string.IsNullOrWhiteSpace(state.Output))
                            Console.WriteLine(state.Output);

                        if (state.IsError && !string.IsNullOrWhiteSpace(state.ErrorMessage))
                            Console.WriteLine($"[ERROR] {state.ErrorMessage}");

                        if (state.ShowMOTD && !string.IsNullOrWhiteSpace(state.MOTD))
                            Console.WriteLine($"[MOTD] {state.MOTD}");

                        continue;
                }
                break; // End loop if terminal phase exits
            }

            Console.WriteLine("Session ended. Bye!");
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
                // Ignore modifier/non-printable keys
                if (char.IsControl(key.KeyChar))
                    continue;
                password += key.KeyChar;
            }
            Console.WriteLine();
            return password;
        }
    }
}
