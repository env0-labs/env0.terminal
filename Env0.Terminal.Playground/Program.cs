﻿using System;
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
                // The outer loop keeps track of session phase (boot, login, terminal)
                switch (state.Phase)
                {
                    case TerminalPhase.Booting:
                        if (state.BootSequenceLines != null)
                        {
                            foreach (var line in state.BootSequenceLines)
                                Console.WriteLine(line);
                        }
                        // Immediately move to login phase—API manages phase switch
                        state = api.Execute(""); // This is expected by your API's design!
                        continue;

                    case TerminalPhase.Login:
                        // Handle either username OR password prompts for local/ssh login
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
                                var password = ReadPassword(); // Hide input
                                if (password == null) continue;
                                state = api.Execute(password);
                            }
                            else
                            {
                                // Defensive: unexpected state, break to outer loop
                                break;
                            }
                        }
                        continue;

                    case TerminalPhase.Terminal:
                    default:
                        // Only print output if present
                        if (!string.IsNullOrWhiteSpace(state.Output))
                        {
                            Console.WriteLine(state.Output);
                            state.Output = null; // Prevent double print
                        }

                        while (true)
                        {
                            Console.Write(state.Prompt);
                            var input = Console.ReadLine();
                            if (input == null) continue;
                            if (input.Trim().ToLower() == "exit") return;

                            state = api.Execute(input);

                            // Handle if the command puts us back into login phase (e.g., SSH, exit, etc)
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
