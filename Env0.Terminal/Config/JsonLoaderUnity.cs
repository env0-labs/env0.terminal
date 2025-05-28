using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
using UnityEngine;
#endif

using Env0.Terminal.Config.Pocos;

namespace Env0.Terminal.Config
{
    public static class JsonLoaderUnity
    {
        public static BootConfig BootConfig { get; private set; }
        public static UserConfig UserConfig { get; private set; }
        public static List<DeviceInfo> Devices { get; private set; } = new List<DeviceInfo>();
        public static Dictionary<string, Pocos.Filesystem> Filesystems { get; private set; } = new Dictionary<string, Pocos.Filesystem>();
        public static List<string> ValidationErrors { get; private set; } = new List<string>();

        private static string LoadJsonFromResources(string resourcePath)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            var textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset != null)
                return textAsset.text;
#endif
            return null;
        }

        public static void LoadAll()
        {
            ValidationErrors.Clear();

            // BootConfig
            BootConfig = LoadBootConfig("Config/Jsons/BootConfig", out var bootErrors);
            ValidationErrors.AddRange(bootErrors);

            // UserConfig
            UserConfig = LoadUserConfig("Config/Jsons/UserConfig", out var userErrors);
            ValidationErrors.AddRange(userErrors);

            // Devices
            Devices = LoadDevices("Config/Jsons/Devices", out var deviceErrors);
            ValidationErrors.AddRange(deviceErrors);

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            DebugUtility.PrintContext("DEBUG", $"[Devices.json] Devices is null? {Devices == null}");
            DebugUtility.PrintContext("DEBUG", $"[Devices.json] Devices count: {(Devices == null ? "null" : Devices.Count.ToString())}");
#endif

            // Filesystems 1-10
            for (var i = 1; i <= 10; i++)
            {
                var filename = $"Filesystem_{i}";
                var resourcePath = $"Config/Jsons/JsonFilesystems/Filesystem_{i}";
                var fs = LoadFilesystemFromResources(resourcePath, out var fsErrors);

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                DebugUtility.PrintContext("DEBUG", $"[{filename}] fs is null? {fs == null}");
                DebugUtility.PrintContext("DEBUG", $"[{filename}] fs.Root is null? {(fs == null ? "n/a" : (fs.Root == null ? "yes" : "no"))}");
#endif

                Filesystems[filename] = fs;
                ValidationErrors.AddRange(fsErrors);
            }
        }

        internal static BootConfig LoadBootConfig(string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJsonFromResources(resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"BootConfig missing: {resourcePath}");
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

        internal static UserConfig LoadUserConfig(string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJsonFromResources(resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"UserConfig missing: {resourcePath} (defaulting to player/password)");
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

        internal static List<DeviceInfo> LoadDevices(string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJsonFromResources(resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"Devices missing: {resourcePath}");
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

        public static Pocos.Filesystem LoadFilesystemFromResources(string resourcePath, out List<string> errors)
        {
            errors = new List<string>();
            var json = LoadJsonFromResources(resourcePath);

            if (string.IsNullOrEmpty(json))
            {
                errors.Add($"Filesystem missing: {resourcePath}");
                return new Pocos.Filesystem();
            }

            try
            {
                var fs = JsonConvert.DeserializeObject<Pocos.Filesystem>(json);

                if (fs?.Root == null || fs.Root.Count == 0)
                {
                    errors.Add($"{resourcePath}: Root is missing or empty.");
                    return new Pocos.Filesystem();
                }

                ValidateEntries(fs.Root, errors, resourcePath, "/");
                return fs;
            }
            catch (Exception ex)
            {
                errors.Add($"{resourcePath} failed to load: {ex.Message}");
                return new Pocos.Filesystem();
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
