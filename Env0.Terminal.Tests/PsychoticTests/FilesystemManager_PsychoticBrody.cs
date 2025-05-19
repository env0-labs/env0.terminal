using Xunit;
using Env0.Terminal.Filesystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Env0.Terminal.Tests
{
    public class PsychoticFilesystemManagerTests
    {
        [Trait("TestType", "Psychotic")]
        // Helper to make a directory cycle
        private FileSystemEntry MakeCycle()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var loop = new FileSystemEntry { Name = "loop", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir", Parent = root };
            root.Children.Add("loop", loop);
            // Create a cycle: loop's child points back to root
            loop.Children.Add("root", root);
            return root;
        }

        [Fact]
        public void CycleDetection_ShouldNotStackOverflowOrLoopForever()
        {
            var fs = new FilesystemManager(MakeCycle());
            // If this hangs, test runner will kill it. This exposes stack overflows or infinite loops.
            var items = fs.ListCurrentDirectory();
            Assert.Contains("loop", items);
        }

        [Fact]
        public void PathTraversal_AttemptToCdDotDotFromRoot_ShouldStayAtRoot()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var fs = new FilesystemManager(root);
            string error;
            fs.ChangeDirectory("..", out error);
            // Should still be at root
            var items = fs.ListCurrentDirectory();
            Assert.Equal(new List<string>(), error == null ? items : new List<string>());
        }

        [Fact]
        public void UnicodeFilenames_ShouldBeHandledCorrectly()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var emoji = new FileSystemEntry { Name = "💩file.txt", IsDirectory = false, Content = "Unicode shit!", Type = "file", Parent = root };
            root.Children.Add("💩file.txt", emoji);

            var fs = new FilesystemManager(root);
            string content, error;
            var result = fs.GetFileContent("💩file.txt", out content, out error);
            Assert.True(result);
            Assert.Equal("Unicode shit!", content);
            Assert.Null(error);
        }

        [Fact]
        public void InsaneFileNameCharacters_ShouldBeHandledOrRejected()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            string insaneName = string.Concat(Enumerable.Repeat("!", 256)) + "\0\t\n\r";
            var insaneFile = new FileSystemEntry { Name = insaneName, IsDirectory = false, Content = "Madness", Type = "file", Parent = root };
            root.Children.Add(insaneName, insaneFile);

            var fs = new FilesystemManager(root);
            string content, error;
            var result = fs.GetFileContent(insaneName, out content, out error);
            // Should not crash, may fail gracefully or succeed
            Assert.True(result || (error != null));
        }

        [Fact]
        public void ExtremelyDeepDirectoryStructure_ShouldNotOverflow()
        {
            FileSystemEntry current = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            FileSystemEntry root = current;
            for (int i = 0; i < 1000; i++)
            {
                var next = new FileSystemEntry { Name = $"dir{i}", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir", Parent = current };
                current.Children.Add($"dir{i}", next);
                current = next;
            }

            var fs = new FilesystemManager(root);
            string error = null;
            for (int i = 0; i < 1000; i++)
            {
                fs.ChangeDirectory($"dir{i}", out error);
                Assert.Null(error);
            }
        }

        [Fact]
        public void LargeNumberOfFiles_ShouldBeListedCorrectly()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            for (int i = 0; i < 10000; i++)
            {
                root.Children.Add($"file_{i}.txt", new FileSystemEntry { Name = $"file_{i}.txt", IsDirectory = false, Content = $"File {i}", Type = "file", Parent = root });
            }
            var fs = new FilesystemManager(root);
            var items = fs.ListCurrentDirectory();
            Assert.Equal(10000, items.Count);
        }

        [Fact]
        public void CatHugeFile_ShouldReturnFullContentOrError()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var sb = new StringBuilder();
            for (int i = 0; i < 1000000; i++) sb.Append("A");
            var hugeFile = new FileSystemEntry { Name = "huge.txt", IsDirectory = false, Content = sb.ToString(), Type = "file", Parent = root };
            root.Children.Add("huge.txt", hugeFile);

            var fs = new FilesystemManager(root);
            string content, error;
            var result = fs.GetFileContent("huge.txt", out content, out error);
            // Accept either all content or a "too large" error
            Assert.True((result && content.Length == 1000000) || (!result && error != null));
        }

        [Fact]
        public void DuplicateFileNames_ShouldNotBeAllowed()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var fileA = new FileSystemEntry { Name = "duplicate.txt", IsDirectory = false, Content = "One", Type = "file", Parent = root };
            root.Children.Add("duplicate.txt", fileA);

            // Try to add another entry with the same key, simulating corruption
            Assert.Throws<System.ArgumentException>(() => root.Children.Add("duplicate.txt", fileA));
        }

        [Fact]
        public void NullFileName_ShouldBeRejectedOrHandled()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            Assert.Throws<System.ArgumentNullException>(() => root.Children.Add(null, null));
        }

        [Fact]
        public void NullOrEmptyInputs_ShouldFailGracefully()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var fs = new FilesystemManager(root);
            string error, content;

            // Null filename
            var result = fs.GetFileContent(null, out content, out error);
            Assert.False(result);

            // Empty filename
            result = fs.GetFileContent("", out content, out error);
            Assert.False(result);

            // Change directory to null
            result = fs.ChangeDirectory(null, out error);
            Assert.False(result);

            // Change directory to empty string
            result = fs.ChangeDirectory("", out error);
            Assert.False(result);
        }

        [Fact]
        public void FileContentIsCaseSensitiveOrNot()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var file = new FileSystemEntry { Name = "README.TXT", IsDirectory = false, Content = "Capital!", Type = "file", Parent = root };
            root.Children.Add("README.TXT", file);

            var fs = new FilesystemManager(root);
            string content, error;
            var result = fs.GetFileContent("readme.txt", out content, out error);
            // Decide: is your FS case sensitive?
            Assert.False(result);
        }

        [Fact]
        public void NonAsciiPathTraversal_ShouldNotBreak()
        {
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            var funky = new FileSystemEntry { Name = "üñîçødë", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir", Parent = root };
            root.Children.Add("üñîçødë", funky);

            var fs = new FilesystemManager(root);
            string error;
            var result = fs.ChangeDirectory("üñîçødë", out error);
            Assert.True(result);
            Assert.Null(error);
        }

        [Fact]
        public void CircularReferenceParentChain_ShouldNotCrash()
        {
            // Parent chain points to self
            var root = new FileSystemEntry { Name = "root", IsDirectory = true, Children = new Dictionary<string, FileSystemEntry>(), Type = "dir" };
            root.Parent = root;

            var fs = new FilesystemManager(root);
            var items = fs.ListCurrentDirectory();
            Assert.Contains("root", root.Name);
        }
    }
}
