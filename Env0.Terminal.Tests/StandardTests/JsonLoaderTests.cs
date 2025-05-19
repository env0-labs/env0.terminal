using System.IO;
using Xunit;
using Env0.Terminal.Config;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Tests
{
    public class JsonLoaderTests
    {
        private const string TestDataDir = "TestData";

        public JsonLoaderTests()
        {
            // Ensure clean slate before each test
            if (!Directory.Exists(TestDataDir))
                Directory.CreateDirectory(TestDataDir);
        }

        [Fact]
        public void LoadBootConfig_ValidJson_LoadsBootText()
        {
            var path = Path.Combine(TestDataDir, "BootConfig_valid.json");
            File.WriteAllText(path, @"{ ""BootText"": [""Line1"", ""Line2""] }");

            var config = JsonLoader.LoadBootConfig(path, out var errors);

            Assert.NotNull(config);
            Assert.Equal(2, config.BootText.Count);
            Assert.Contains("Line1", config.BootText);
            Assert.Empty(errors);
        }

        [Fact]
        public void LoadBootConfig_FileMissing_ReportsError()
        {
            var path = Path.Combine(TestDataDir, "Nonexistent.json");
            if (File.Exists(path)) File.Delete(path);

            var config = JsonLoader.LoadBootConfig(path, out var errors);

            Assert.Null(config);
            Assert.Contains(errors, e => e.ToLower().Contains("missing"));
        }

        [Fact]
        public void LoadBootConfig_EmptyBootText_ReportsError()
        {
            var path = Path.Combine(TestDataDir, "BootConfig_empty.json");
            File.WriteAllText(path, @"{ ""BootText"": [] }");

            var config = JsonLoader.LoadBootConfig(path, out var errors);

            Assert.NotNull(config);
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ToLower().Contains("empty"));
        }

        [Fact]
        public void LoadBootConfig_MalformedJson_ReportsError()
        {
            var path = Path.Combine(TestDataDir, "BootConfig_bad.json");
            File.WriteAllText(path, @"{ this is not: valid json }");

            var config = JsonLoader.LoadBootConfig(path, out var errors);

            Assert.Null(config);
            Assert.Contains(errors, e => e.ToLower().Contains("failed to load"));
        }
    }
}
