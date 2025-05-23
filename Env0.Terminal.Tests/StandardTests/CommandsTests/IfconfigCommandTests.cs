using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;

namespace Env0.Terminal.Tests.Commands
{
    public class IfconfigCommandTests
    {
        /// <summary>
        /// IfconfigCommand returns "Not implemented yet." for now.
        /// </summary>
        [Fact]
        public void IfconfigCommand_Stubbed_ReturnsNotImplemented()
        {
            var cmd = new IfconfigCommand();
            var session = new SessionState();
            var result = cmd.Execute(session, new string[0]);

            Assert.True(result.IsError);
            Assert.Contains("Not implemented yet", result.Output);
        }
    }
}