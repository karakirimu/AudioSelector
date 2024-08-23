using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AudioSelector.Setting
{

    public class AppConfigProperty
    {
        public SystemTheme theme;
        public ushort hotkey_id;
        public List<string> hotkey;
    }

    internal class AppConfig
    {
        public AppConfigProperty Property { get; set; }

        private readonly string ConfigFolder;

        private readonly string ConfigFileName = "config.json";

        public AppConfig() {
            string AppDataPath= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            ConfigFolder = Path.Combine(AppDataPath, "AudioSelector");

        }
        public void Load()
        {
            // Get the path of the appdata folder
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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
                using var fileStream = File.Create(configFilePath);

                // Create the data object
                var data = new AppConfigProperty
                {
                    theme = SystemTheme.Invalid,
                    hotkey_id = 0x2652,
                    hotkey = [""]
                };

                JsonSerializer.Serialize(fileStream, data);
            }
            else
            {
                // Load the existing JSON file
                using var fileStream = File.OpenRead(configFilePath);

                // Deserialize the JSON data
                Property = JsonSerializer.Deserialize<AppConfigProperty>(fileStream);
            }
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
