using System;
using System.IO;
using System.Text.Json;
using static AudioSelector.Setting.AppConfig;

namespace AudioSelector.Setting
{
    public enum AppConfigType
    {
        Theme,
        Language,
        HotKeyEnabled,
        HotKeyId,
        HotKey,
        Startup
    }

    public interface IAppConfig
    {
        public AppConfigProperty Property { get; }
        public event ValueUpdate UserConfigurationUpdate;
        public void SetTheme(SystemTheme theme);
        public void SetLanguage(string language);
        public void SetHotKeyEnabled(bool enabled);
        public void SetHotkeyId(ushort hotkeyId);
        public void SetHotkey(HotKey hotkey);
        public void SetStartup(bool enabled);
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

                // Create the default data object
                JsonSerializer.Serialize(writer, AppJsonFormat.CreateLatest());
            }

            // Load the existing JSON file
            using var reader = File.OpenRead(configFilePath);

            // Deserialize the JSON data
            Property = JsonSerializer.Deserialize<AppConfigProperty>(reader);

            // Update Property if needed
            Property = AppJsonFormat.Update(Property);

            if(Property != null)
            {
                bool startup = StartupStoreApp.IsStoreApp() ? StartupStoreApp.CheckStartupEntry().Result : SystemRegistry.HasStartupEntry();
                if(Property.Startup != startup)
                {
                    reader.Close();
                    SetStartup(startup);
                }
            }
        }

        public void SetTheme(SystemTheme theme)
        {
            Property.Theme = theme;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.Theme, Property);
        }

        public void SetLanguage(string language)
        { 
            Property.Language = language;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.Language, Property);
        }

        public void SetHotKeyEnabled(bool enabled)
        {
            Property.Hotkey_enabled = enabled;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.HotKeyEnabled, Property);
        }

        public void SetHotkeyId(ushort hotkeyId)
        {
            Property.Hotkey_id = hotkeyId;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.HotKeyId, Property);
        }

        public void SetHotkey(HotKey hotkey)
        {
            Property.Hotkey = hotkey;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.HotKey, Property);
        }
        public void SetStartup(bool enabled)
        {
            Property.Startup = enabled;
            Save();
            UserConfigurationUpdate?.Invoke(AppConfigType.Startup, Property);
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
