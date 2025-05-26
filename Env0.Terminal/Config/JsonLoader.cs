using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Env0.Terminal.Config.Pocos;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
using UnityEngine;
#endif

namespace Env0.Terminal.Config
{
    public static class JsonLoader
    {
        // Loaded configs
        public static BootConfig BootConfig { get; private set; }
        public static UserConfig UserConfig { get; private set; }

        // Devices
        public static List<DeviceInfo> Devices { get; private set; } = new();

        // Filesystems (keyed by filename, e.g., "Filesystem_1.json")
        public static Dictionary<string, Env0.Terminal.Config.Pocos.Filesystem> Filesystems { get; private set; } = new();

        // Validation errors (visible in debug)
        public static List<string> ValidationErrors { get; private set; } = new List<string>();

        /// <summary>
        /// Loads JSON from either Resources (Unity) or direct file path (CLI).
        /// </summary>
        private static string LoadJson(string filePath, string resourcePathWithoutExtension)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            var textAsset = Resources.Load<TextAsset>(resourcePathWithoutExtension);
            if (textAsset != null)
                return textAsset.text;
#endif
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            return null;
        }

        public static void LoadAll()
        {
            ValidationErrors.Clear();

            // BootConfig
            BootConfig = LoadBootConfig(
                "Config/Jsons/BootConfig.json",
                "Config/Jsons/BootConfig",
                out var bootErrors
            );
            ValidationErrors.AddRange(bootErrors);

            // UserConfig
            UserConfig = LoadUserConfig(
                "Config/Jsons/UserConfig.json",
                "Config/Jsons/UserConfig",
                out var userErrors
            );
            ValidationErrors.AddRange(userErrors);

            // Devices
            Devices = LoadDevices(
                "Config/Jsons/Devices.json",
                "Config/Jsons/Devices",
                out var deviceErrors
            );
            ValidationErrors.AddRange(deviceErrors);

            // Filesystems 1-10
            for (int i = 1; i <= 10; i++)
            {
                var filename = $"Filesystem_{i}.json";
                var filePath = $"Config/Jsons/JsonFilesystems/{filename}";
                var resourcePath = $"Config/Jsons/JsonFilesystems/Filesystem_{i}";

                // Debug 1: Print raw JSON before deserialization
                var raw = LoadJson(filePath, resourcePath);
                if (raw != null)
                {
                    DebugUtility.PrintContext("FS", $"==== RAW JSON FOR {filePath} / {resourcePath} ====");
                    DebugUtility.PrintContext("FS", raw);
                    DebugUtility.PrintContext("FS", "==================");
                }

                var fs = LoadFilesystem(filePath, resourcePath, out var fsErrors);

                // Debug 2: Print fs.Root state after deserialization
                if (fs != null)
                {
                    DebugUtility.PrintContext("FS", $"fs.Root is null? {(fs.Root == null)}");
                    if (fs.Root != null)
                        DebugUtility.PrintContext("FS", $"fs.Root keys: {string.Join(", ", fs.Root.Keys)}");
                    else
                        DebugUtility.PrintContext("FS", "fs.Root keys: null");

                    // Debug 3: Print tutorial.txt content if present
                    if (fs.Root != null && fs.Root.ContainsKey("tutorial.txt"))
                        DebugUtility.PrintContext("FS", $"tutorial.txt content (from loader): {fs.Root["tutorial.txt"].Content ?? "NULL"}");
                    else
                        DebugUtility.PrintContext("FS", "tutorial.txt not found in fs.Root!");
                }

                Filesystems[filename] = fs;
                ValidationErrors.AddRange(fsErrors);
            }

            // Filesystem_11.json (safe fallback)
            var safeFile = "Config/Jsons/JsonFilesystems/Filesystem_11.json";
            var safeResource = "Config/Jsons/JsonFilesystems/Filesystem_11";
            var safeRaw = LoadJson(safeFile, safeResource);
            if (safeRaw != null)
            {
                DebugUtility.PrintContext("FS", $"==== RAW JSON FOR {safeFile} / {safeResource} ====");
                DebugUtility.PrintContext("FS", safeRaw);
                DebugUtility.PrintContext("FS", "==================");
            }

            var safeFs = LoadFilesystem(safeFile, safeResource, out var safeErrors);

            if (safeFs != null)
            {
                DebugUtility.PrintContext("FS", $"safeFs.Root is null? {safeFs.Root == null}");
                if (safeFs.Root != null)
                    DebugUtility.PrintContext("FS", $"safeFs.Root keys: {string.Join(", ", safeFs.Root.Keys)}");
                else
                    DebugUtility.PrintContext("FS", "safeFs.Root keys: null");
            }

            Filesystems["Filesystem_11.json"] = safeFs;
            ValidationErrors.AddRange(safeErrors);
        }

        internal static BootConfig LoadBootConfig(string filePath, string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJson(filePath, resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"BootConfig missing: {filePath} / {resourcePath}");
                return null;
            }
            try
            {
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

        internal static UserConfig LoadUserConfig(string filePath, string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJson(filePath, resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"UserConfig missing: {filePath} / {resourcePath} (defaulting to player/password)");
                return new UserConfig { Username = "player", Password = "password" };
            }

            try
            {
                var config = JsonConvert.DeserializeObject<UserConfig>(json);

                if (string.IsNullOrWhiteSpace(config?.Username) || string.IsNullOrWhiteSpace(config?.Password))
                {
                    errors.Add("UserConfig: username or password is missing or empty (defaulting to player/password)");
                    return new UserConfig { Username = "player", Password = "password" };
                }

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

        internal static List<DeviceInfo> LoadDevices(string filePath, string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJson(filePath, resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"Devices missing: {filePath} / {resourcePath}");
                return new List<DeviceInfo>();
            }

            try
            {
                var devices = JsonConvert.DeserializeObject<List<DeviceInfo>>(json);

                if (devices == null || devices.Count == 0)
                {
                    errors.Add("Devices.json is empty or invalid.");
                    return new List<DeviceInfo>();
                }

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    if (string.IsNullOrWhiteSpace(device.Ip)) errors.Add($"Device {i}: IP missing.");
                    if (string.IsNullOrWhiteSpace(device.Hostname)) errors.Add($"Device {i}: Hostname missing.");
                    if (string.IsNullOrWhiteSpace(device.Mac)) errors.Add($"Device {i}: MAC missing.");
                    if (string.IsNullOrWhiteSpace(device.Username)) errors.Add($"Device {i}: Username missing.");
                    if (string.IsNullOrWhiteSpace(device.Password)) errors.Add($"Device {i}: Password missing.");
                    if (string.IsNullOrWhiteSpace(device.Filesystem)) errors.Add($"Device {i}: Filesystem missing.");
                    if (device.Interfaces == null || device.Interfaces.Count == 0) errors.Add($"Device {i}: No interfaces defined.");
                    if (device.Ports == null) device.Ports = new List<string>();
                    if (string.IsNullOrWhiteSpace(device.Motd)) device.Motd = $"Welcome to {device.Hostname}";

                    if (device.Interfaces != null)
                    {
                        for (int j = 0; j < device.Interfaces.Count; j++)
                        {
                            var iface = device.Interfaces[j];
                            if (string.IsNullOrWhiteSpace(iface.Name)) errors.Add($"Device {i} Interface {j}: Name missing.");
                            if (string.IsNullOrWhiteSpace(iface.Ip)) errors.Add($"Device {i} Interface {j}: IP missing.");
                            if (string.IsNullOrWhiteSpace(iface.Subnet)) errors.Add($"Device {i} Interface {j}: Subnet missing.");
                            if (string.IsNullOrWhiteSpace(iface.Mac)) errors.Add($"Device {i} Interface {j}: MAC missing.");
                        }
                    }
                }

                return devices;
            }
            catch (Exception ex)
            {
                errors.Add($"Devices failed to load: {ex.Message}");
                return new List<DeviceInfo>();
            }
        }

        public static Env0.Terminal.Config.Pocos.Filesystem LoadFilesystem(string filePath, string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJson(filePath, resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"Filesystem missing: {filePath} / {resourcePath}");
                return new Env0.Terminal.Config.Pocos.Filesystem(); // Safe empty FS
            }

            try
            {
                var fs = JsonConvert.DeserializeObject<Env0.Terminal.Config.Pocos.Filesystem>(json);

                if (fs?.Root == null || fs.Root.Count == 0)
                {
                    errors.Add($"{Path.GetFileName(filePath)}: Root is missing or empty.");
                    return new Env0.Terminal.Config.Pocos.Filesystem();
                }

                ValidateEntries(fs.Root, errors, Path.GetFileName(filePath), "/");
                return fs;
            }
            catch (Exception ex)
            {
                errors.Add($"{Path.GetFileName(filePath)} failed to load: {ex.Message}");
                return new Env0.Terminal.Config.Pocos.Filesystem();
            }
        }

        private static void ValidateEntries(Dictionary<string, FileEntry> entries, List<string> errors, string fsName, string path)
        {
            foreach (var kvp in entries)
            {
                var name = kvp.Key;
                var entry = kvp.Value;

                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add($"{fsName}: Empty or invalid entry name at {path}");
                    continue;
                }

                if (entry.Type == "file")
                {
                    if (entry.Content == null)
                        errors.Add($"{fsName}: File '{name}' at {path} missing content.");
                    if (entry.Children != null)
                        errors.Add($"{fsName}: File '{name}' at {path} should not have children.");
                }
                else
                {
                    if (entry.Children == null)
                        errors.Add($"{fsName}: Directory '{name}' at {path} missing children dictionary.");
                    else
                        ValidateEntries(entry.Children, errors, fsName, path + name + "/");
                }
            }
        }

        private static bool IsAscii(string value)
        {
            return !string.IsNullOrEmpty(value) && value.All(c => c <= 127);
        }
    }
}
