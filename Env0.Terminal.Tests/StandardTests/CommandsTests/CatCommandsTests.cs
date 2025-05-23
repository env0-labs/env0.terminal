using Xunit;
using Env0.Terminal.Terminal;
using Env0.Terminal.Terminal.Commands;
using Env0.Terminal.Filesystem;

namespace Env0.Terminal.Tests.Commands
{
    public class CatCommandTests
    {
        /// <summary>
        /// CatCommand should return an error for missing or directory target.
        /// </summary>
        [Fact]
        public void CatCommand_MissingOrDir_ReturnsError()
        {
            var root = new FileSystemEntry { Name = "/", IsDirectory = true };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new string[0]);
            Assert.True(result.IsError);

            result = cmd.Execute(session, new[] { "/" });
            Assert.True(result.IsError);
        }

        /// <summary>
        /// CatCommand returns file contents when present.
        /// </summary>
        [Fact]
        public void CatCommand_File_ReturnsContents()
        {
            var file = new FileSystemEntry { Name = "foo.txt", IsDirectory = false, Content = "hello" };
            var root = new FileSystemEntry { Name = "/", IsDirectory = true, Children = { { "foo.txt", file } } };
            var session = new SessionState { FilesystemManager = new FilesystemManager(root) };
            var cmd = new CatCommand();

            var result = cmd.Execute(session, new[] { "foo.txt" });
            Assert.False(result.IsError);
            Assert.Equal("hello", result.Output);
        }
    }
}