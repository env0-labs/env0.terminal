using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;

namespace Env0.Terminal.Tests.Commands
{
    public class SshCommandTests
    {
        /// <summary>
        /// SshCommand returns "Not implemented yet." for now.
        /// </summary>
        [Fact]
        public void SshCommand_Stubbed_ReturnsNotImplemented()
        {
            var cmd = new SshCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new[] { "otherhost" });

            Assert.True(result.IsError);
            Assert.Contains("Not implemented yet", result.Output);
        }
    }
}