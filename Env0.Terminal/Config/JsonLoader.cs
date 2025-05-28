using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Config
{
    public static class JsonLoader
    {
        // Loaded configs
        public static BootConfig BootConfig { get; private set; }
        public static UserConfig UserConfig { get; private set; }
        public static List<DeviceInfo> Devices { get; private set; } = new List<DeviceInfo>();
        public static Dictionary<string, Pocos.Filesystem> Filesystems { get; private set; } = new Dictionary<string, Pocos.Filesystem>();
        public static List<string> ValidationErrors { get; private set; } = new List<string>();

        public static void LoadAll()
        {
            ValidationErrors.Clear();

            // BootConfig
            BootConfig = LoadBootConfig("Config/Jsons/BootConfig.json", out var bootErrors);
            ValidationErrors.AddRange(bootErrors);

            // UserConfig
            UserConfig = LoadUserConfig("Config/Jsons/UserConfig.json", out var userErrors);
            ValidationErrors.AddRange(userErrors);

            // Devices
            Devices = LoadDevices("Config/Jsons/Devices.json", out var deviceErrors);
            ValidationErrors.AddRange(deviceErrors);
            DebugUtility.PrintContext("DEBUG", $"[Devices.json] Devices is null? {Devices == null}");
            DebugUtility.PrintContext("DEBUG", $"[Devices.json] Devices count: {(Devices == null ? "null" : Devices.Count.ToString())}");

            // Filesystems 1-10
            for (var i = 1; i <= 10; i++)
            {
                var filename = $"Filesystem_{i}.json";
                var path = $"Config/Jsons/JsonFilesystems/{filename}";
                var fs = LoadFilesystem(path, out var fsErrors);

                // Debug output for this filesystem
                DebugUtility.PrintContext("DEBUG", $"[{filename}] fs is null? {fs == null}");
                DebugUtility.PrintContext("DEBUG", $"[{filename}] fs.Root is null? {(fs == null ? "n/a" : (fs.Root == null ? "yes" : "no"))}");

                Filesystems[filename] = fs;
                ValidationErrors.AddRange(fsErrors);
            }
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

                if (string.IsNullOrWhiteSpace(config?.Username) || string.IsNullOrWhiteSpace(config?.Password))
                {
                    errors.Add("UserConfig: username or password is missing or empty (defaulting to player/password)");
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

        internal static List<DeviceInfo> LoadDevices(string path, out List<string> errors)
        {
            errors = new List<string>();
            if (!File.Exists(path))
            {
                errors.Add($"Devices missing: {path}");
                return new List<DeviceInfo>();
            }
            try
            {
                var json = File.ReadAllText(path);
                var devices = JsonConvert.DeserializeObject<List<DeviceInfo>>(json);

                if (devices == null || devices.Count == 0)
                {
                    errors.Add("Devices.json is empty or invalid.");
                    return new List<DeviceInfo>();
                }
                return devices;
            }
            catch (Exception ex)
            {
                errors.Add($"Devices failed to load: {ex.Message}");
                return new List<DeviceInfo>();
            }
        }

        public static Pocos.Filesystem LoadFilesystem(string path, out List<string> errors)
        {
            errors = new List<string>();
            if (!File.Exists(path))
            {
                errors.Add($"Filesystem missing: {path}");
                return new Pocos.Filesystem();
            }
            try
            {
                var json = File.ReadAllText(path);
                var fs = JsonConvert.DeserializeObject<Pocos.Filesystem>(json);

                if (fs?.Root == null || fs.Root.Count == 0)
                {
                    errors.Add($"{path}: Root is missing or empty.");
                    return new Pocos.Filesystem();
                }
                return fs;
            }
            catch (Exception ex)
            {
                errors.Add($"{path} failed to load: {ex.Message}");
                return new Pocos.Filesystem();
            }
        }
    }
}
