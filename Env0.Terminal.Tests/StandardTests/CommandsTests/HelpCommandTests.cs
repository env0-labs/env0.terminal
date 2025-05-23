using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;

namespace Env0.Terminal.Tests.Commands
{
    public class HelpCommandTests
    {
        /// <summary>
        /// HelpCommand returns "Not implemented yet." for now.
        /// </summary>
        [Fact]
        public void HelpCommand_Stubbed_ReturnsNotImplemented()
        {
            var cmd = new HelpCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new string[0]);

            Assert.True(result.IsError);
            Assert.Contains("Not implemented yet", result.Output);
        }
    }
}