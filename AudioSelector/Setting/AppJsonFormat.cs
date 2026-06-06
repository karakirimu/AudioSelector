using System.Text.Json.Serialization;
using System.Windows.Input;

namespace AudioSelector.Setting
{
    public enum TrayDoubleClickTarget
    {
        Speaker,
        Microphone
    }

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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? LegacyHotkeyEnabled { get; set; }

        [JsonPropertyName("hotkey_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ushort? LegacyHotkeyId { get; set; }

        [JsonPropertyName("hotkey")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HotKey LegacyHotkey { get; set; }

        [JsonPropertyName("speaker_hotkey_enabled")]
        public bool SpeakerHotkeyEnabled { get; set; }

        [JsonPropertyName("speaker_hotkey_id")]
        public ushort SpeakerHotkeyId { get; set; }

        [JsonPropertyName("speaker_hotkey")]
        public HotKey SpeakerHotkey { get; set; }

        [JsonPropertyName("microphone_hotkey_enabled")]
        public bool MicrophoneHotkeyEnabled { get; set; }

        [JsonPropertyName("microphone_hotkey_id")]
        public ushort MicrophoneHotkeyId { get; set; }

        [JsonPropertyName("microphone_hotkey")]
        public HotKey MicrophoneHotkey { get; set; }

        [JsonPropertyName("tray_double_click_target")]
        public TrayDoubleClickTarget TrayDoubleClickTarget { get; set; }

        [JsonPropertyName("startup")]
        public bool Startup { get; set; }
    }

    public class AppJsonFormat
    {
        private const string LatestVersion = "1.3.0";

        public static AppConfigProperty CreateLatest()
        {
            // Create the default data object
            var data = new AppConfigProperty
            {
                Version = LatestVersion,
                Theme = SystemTheme.System,
                Language = "System",
                SpeakerHotkeyEnabled = true,
                SpeakerHotkeyId = 0x2652,
                SpeakerHotkey = CreateSpeakerHotkey(),
                MicrophoneHotkeyEnabled = true,
                MicrophoneHotkeyId = 0x2653,
                MicrophoneHotkey = CreateMicrophoneHotkey(),
                TrayDoubleClickTarget = TrayDoubleClickTarget.Speaker,
                Startup = StartupStoreApp.IsStoreApp() ? StartupStoreApp.CheckStartupEntry().Result : SystemRegistry.HasStartupEntry()
            };

            return data;
        }

        public static AppConfigProperty Update(AppConfigProperty property)
        {
            if (property == null)
            {
                return CreateLatest();
            }

            // V1.1.3 -> V1.2.0
            if(property.Version == null)
            {
                property.Version = "1.0.0";
                property.Theme = (SystemTheme)(((int)property.Theme + 1) % 3);
                property.Language = "System";
            }

            // V1.2.0 -> V1.3.0
            if (property.Version != LatestVersion)
            {
                property.SpeakerHotkeyEnabled = property.LegacyHotkeyEnabled ?? true;
                property.SpeakerHotkeyId = property.LegacyHotkeyId ?? 0x2652;
                property.SpeakerHotkey = property.LegacyHotkey ?? CreateSpeakerHotkey();
                property.MicrophoneHotkeyEnabled = true;
                property.MicrophoneHotkeyId = 0x2653;
                property.MicrophoneHotkey = CreateMicrophoneHotkey();
                property.TrayDoubleClickTarget = TrayDoubleClickTarget.Speaker;
                property.LegacyHotkeyEnabled = null;
                property.LegacyHotkeyId = null;
                property.LegacyHotkey = null;
                property.Version = LatestVersion;
            }

            property.SpeakerHotkey ??= CreateSpeakerHotkey();
            property.MicrophoneHotkey ??= CreateMicrophoneHotkey();
            property.SpeakerHotkeyId = property.SpeakerHotkeyId == 0 ? (ushort)0x2652 : property.SpeakerHotkeyId;
            property.MicrophoneHotkeyId = property.MicrophoneHotkeyId == 0 ? (ushort)0x2653 : property.MicrophoneHotkeyId;

            return property;
        }

        private static HotKey CreateSpeakerHotkey()
        {
            return new HotKey()
            {
                Win = false,
                Ctrl = true,
                Shift = false,
                Alt = true,
                VirtualKey = Key.V.ToString()
            };
        }

        private static HotKey CreateMicrophoneHotkey()
        {
            return new HotKey()
            {
                Win = false,
                Ctrl = true,
                Shift = false,
                Alt = true,
                VirtualKey = Key.N.ToString()
            };
        }
    }
}
