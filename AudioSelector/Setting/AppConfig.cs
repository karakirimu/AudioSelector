using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using static AudioSelector.Setting.AppConfig;

namespace AudioSelector.Setting
{
    public class HotKey
    {
        [JsonPropertyName("win")]
        public bool Win { get; set; }

        [JsonPropertyName("ctrl")]
        public bool Ctrl { get; set; }

        [JsonPropertyName("shift")]
        public bool Shift { get; set; }

        [JsonPropertyName("alt")]
        public bool Alt { get; set; }

        [JsonPropertyName("virtual_key")]
        public string VirtualKey { get; set; }
    }

    public class AppConfigProperty
    {
        [JsonPropertyName("theme")]
        public SystemTheme Theme { get; set; }

        [JsonPropertyName("hotkey_id")]
        public ushort Hotkey_id { get; set; }

        [JsonPropertyName("hotkey")]
        public HotKey Hotkey { get; set; }
    }

    public enum AppConfigType
    {
        Theme,
        HotkeyId,
        HotKey
    }

    public interface IAppConfig
    {
        public AppConfigProperty Property { get; }
        public event ValueUpdate UserConfigurationUpdate;
        public void SetTheme(SystemTheme theme);
        public void SetHotkeyId(ushort hotkeyId);
        public void SetHotkey(HotKey hotkey);
    }

    public class AppConfig : IAppConfig
    {
        public AppConfigProperty Property { get; private set; }
        private readonly string ConfigFolder;
        private readonly string ConfigFileName = "config.json";

        public delegate void ValueUpdate(AppConfigType type, AppConfigProperty property);
        public event ValueUpdate UserConfigurationUpdate;

        public AppConfig()
        {
            string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            ConfigFolder = Path.Combine(AppDataPath, "AudioSelector");
        }

        public void Load()
        {
            // Create the AudioSelector folder
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            // Create the config.json file path
            string configFilePath = Path.Combine(ConfigFolder, ConfigFileName);

            // Check if the JSON file exists
            if (!File.Exists(configFilePath))
            {
                // If the JSON file doesn't exist, create it
                using var writer = File.Create(configFilePath);

                // Create the data object
                var data = new AppConfigProperty
                {
                    Theme = SystemTheme.System,
                    Hotkey_id = 0x2652,
                    Hotkey = new HotKey()
                    {
                        Win = false,
                        Ctrl = true,
                        Shift = false,
                        Alt = true,
                        VirtualKey = Key.V.ToString()
                    }
                };

                JsonSerializer.Serialize(writer, data);
            }

            // Load the existing JSON file
            using var reader = File.OpenRead(configFilePath);

            // Deserialize the JSON data
            Property = JsonSerializer.Deserialize<AppConfigProperty>(reader);
        }

        public void SetTheme(SystemTheme theme)
        {
            Property.Theme = theme;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.Theme, Property);
        }

        public void SetHotkeyId(ushort hotkeyId)
        {
            Property.Hotkey_id = hotkeyId;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.HotkeyId, Property);
        }

        public void SetHotkey(HotKey hotkey)
        {
            Property.Hotkey = hotkey;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.HotKey, Property);
        }

        public void Save()
        {
            string configFilePath = Path.Combine(ConfigFolder, ConfigFileName);

            // Serialize the data object
            using var fileStream = File.Create(configFilePath);
            JsonSerializer.Serialize(fileStream, Property);
        }
    }
}
