using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Config
{
    public static class JsonLoader
    {
        public static BootConfig BootConfig { get; private set; }
        public static List<string> ValidationErrors { get; private set; } = new List<string>();

        public static void LoadAll()
        {
            ValidationErrors.Clear();

            // Load BootConfig.json
            BootConfig = LoadBootConfig("Env0.Terminal/Config/Jsons/BootConfig.json", out var bootErrors);
            ValidationErrors.AddRange(bootErrors);

            // TODO: Implement and call UserConfig, Devices, Filesystems as you build those models/loaders.

            // No cross-validation needed until those are present.
        }

            internal static BootConfig LoadBootConfig(string path, out List<string> errors)
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
    }
}