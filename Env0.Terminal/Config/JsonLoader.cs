using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Needed for .All() in IsAscii
using Newtonsoft.Json;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Config
{
    public static class JsonLoader
    {
        // Loaded configs
        public static BootConfig? BootConfig { get; private set; }
        public static UserConfig? UserConfig { get; private set; }

        // Validation errors (visible in debug)
        public static List<string> ValidationErrors { get; private set; } = new List<string>();

        public static void LoadAll()
        {
            ValidationErrors.Clear();

            // Load BootConfig.json
            BootConfig = LoadBootConfig("Env0.Terminal/Config/Jsons/BootConfig.json", out var bootErrors);
            ValidationErrors.AddRange(bootErrors);

            // Load UserConfig.json
            UserConfig = LoadUserConfig("Env0.Terminal/Config/Jsons/UserConfig.json", out var userErrors);
            ValidationErrors.AddRange(userErrors);

            // TODO: Add Devices and Filesystem loaders here as you implement them.
        }

        internal static BootConfig? LoadBootConfig(string path, out List<string> errors)
        {
            errors = new List<string>();
            if (!File.Exists(path))
            {
                errors.Add($"BootConfig missing: {path}");
                return null;
            }
            try
            {
                var json = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<BootConfig>(json);
                if (config?.BootText == null || config.BootText.Count == 0)
                    errors.Add("BootConfig: BootText is missing or empty.");
                return config;
            }
            catch (Exception ex)
            {
                errors.Add($"BootConfig failed to load: {ex.Message}");
                return null;
            }
        }

        internal static UserConfig LoadUserConfig(string path, out List<string> errors)
        {
            errors = new List<string>();

            if (!File.Exists(path))
            {
                errors.Add($"UserConfig missing: {path} (defaulting to player/password)");
                return new UserConfig { Username = "player", Password = "password" };
            }

            try
            {
                var json = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<UserConfig>(json);

                // Null/empty checks
                if (string.IsNullOrWhiteSpace(config?.Username) || string.IsNullOrWhiteSpace(config?.Password))
                {
                    errors.Add("UserConfig: username or password is missing or empty (defaulting to player/password)");
                    return new UserConfig { Username = "player", Password = "password" };
                }

                // ASCII check
                if (!IsAscii(config.Username) || !IsAscii(config.Password))
                {
                    errors.Add("UserConfig: username or password contains non-ASCII characters (defaulting to player/password)");
                    return new UserConfig { Username = "player", Password = "password" };
                }

                return config;
            }
            catch (Exception ex)
            {
                errors.Add($"UserConfig failed to load: {ex.Message} (defaulting to player/password)");
                return new UserConfig { Username = "player", Password = "password" };
            }
        }

        private static bool IsAscii(string value)
        {
            // Check for null/empty just in case
            return !string.IsNullOrEmpty(value) && value.All(c => c <= 127);
        }
    }
}
