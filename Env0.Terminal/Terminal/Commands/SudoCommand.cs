using System;
using Env0.Terminal.Terminal;

namespace Env0.Terminal.Terminal.Commands
{
    public class SudoCommand : ICommand
    {
        private static readonly string[] Responses = new[]
        {
            "Nice try.",
            "You must be joking.",
            "Permission denied. (And no, sudo won't help.)",
            "Sudo is for real admins only.",
            "Not on this terminal, friend.",
            "No power for you.",
            "Dream on.",
            "Are you feeling lucky?",
            "Denied. (But I appreciate the ambition.)",
            "Not even root gets that.",
            "Come back with real credentials.",
            "You wish.",
            "No soup for you.",
            "Try again in another life.",
            "This isn't that kind of party."
        };

        public CommandResult Execute(SessionState session, string[] args)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            var response = Responses[random.Next(Responses.Length)];
            return new CommandResult(response + "\n");
        }
    }
}