using System.Text.Json.Serialization;
using System.Windows.Input;

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
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("theme")]
        public SystemTheme Theme { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("hotkey_enabled")]
        public bool Hotkey_enabled { get; set; }

        [JsonPropertyName("hotkey_id")]
        public ushort Hotkey_id { get; set; }

        [JsonPropertyName("hotkey")]
        public HotKey Hotkey { get; set; }

        [JsonPropertyName("startup")]
        public bool Startup { get; set; }
    }

    public class AppJsonFormat
    {
        public static AppConfigProperty CreateLatest()
        {
            // Create the default data object
            var data = new AppConfigProperty
            {
                Version = "1.0.0",
                Theme = SystemTheme.System,
                Language = "System",
                Hotkey_enabled = true,
                Hotkey_id = 0x2652,
                Hotkey = new HotKey()
                {
                    Win = false,
                    Ctrl = true,
                    Shift = false,
                    Alt = true,
                    VirtualKey = Key.V.ToString()
                },
                Startup = StartupStoreApp.IsStoreApp() ? StartupStoreApp.CheckStartupEntry().Result : SystemRegistry.HasStartupEntry()
            };

            return data;
        }

        public static AppConfigProperty Update(AppConfigProperty property)
        {
            // V1.1.3 -> V1.2.0
            if(property.Version == null)
            {
                property.Version = "1.0.0";
                property.Theme = (SystemTheme)(((int)property.Theme + 1) % 3);
                property.Language = "System";
            }

            return property;
        }
    }
}
