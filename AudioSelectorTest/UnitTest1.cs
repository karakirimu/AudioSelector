using AudioSelector.Setting;
using System.Text.Json;

namespace AudioSelectorTest
{
    public class Tests
    {
        /// <summary>
        /// Version 1.1.3 default config
        /// </summary>
        private const string config113 = "{" +
            "\"theme\":2," +
            "\"hotkey_enabled\":true," +
            "\"hotkey_id\":9810," +
            "\"hotkey\":{" +
            "\"win\":false," +
            "\"ctrl\":true," +
            "\"shift\":false," +
            "\"alt\":true," +
            "\"virtual_key\":\"V\"" +
            "}," +
            "\"startup\":false}";

        /// <summary>
        /// Version 1.2.0 default config
        /// </summary>
        private const string config120 = "{" +
            "\"version\":\"1.0.0\"," +
            "\"theme\":0," +
            "\"language\":\"System\"," +
            "\"hotkey_enabled\":true," +
            "\"hotkey_id\":9810," +
            "\"hotkey\":{" +
            "\"win\":false," +
            "\"ctrl\":true," +
            "\"shift\":false," +
            "\"alt\":true," +
            "\"virtual_key\":\"V\"" +
            "}," +
            "\"startup\":false}";

        /// <summary>
        /// Version 1.3.0 default config
        /// </summary>
        private const string config130 = "{" +
            "\"version\":\"1.3.0\"," +
            "\"theme\":0," +
            "\"language\":\"System\"," +
            "\"speaker_hotkey_enabled\":true," +
            "\"speaker_hotkey_id\":9810," +
            "\"speaker_hotkey\":{" +
            "\"win\":false," +
            "\"ctrl\":true," +
            "\"shift\":false," +
            "\"alt\":true," +
            "\"virtual_key\":\"V\"" +
            "}," +
            "\"microphone_hotkey_enabled\":true," +
            "\"microphone_hotkey_id\":9811," +
            "\"microphone_hotkey\":{" +
            "\"win\":false," +
            "\"ctrl\":true," +
            "\"shift\":false," +
            "\"alt\":true," +
            "\"virtual_key\":\"N\"" +
            "}," +
            "\"tray_double_click_target\":0," +
            "\"startup\":false}";


        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Version 1.1.3 to 1.2.0 Setting Update
        /// </summary>
        [Test]
        public void UpdateFrom113()
        {
            // Deserialize the JSON data
            var Property = JsonSerializer.Deserialize<AppConfigProperty>(config113);
            var updatedProperty = AppJsonFormat.Update(Property);

            Assert.Multiple(() =>
            {
                Assert.That(updatedProperty.Version, Is.EqualTo("1.3.0"));
                Assert.That(updatedProperty.Theme, Is.EqualTo(SystemTheme.System));
                Assert.That(updatedProperty.Language, Is.EqualTo("System"));
                Assert.That(updatedProperty.SpeakerHotkeyEnabled, Is.EqualTo(true));
                Assert.That(updatedProperty.SpeakerHotkeyId, Is.EqualTo(9810));
                Assert.That(updatedProperty.SpeakerHotkey.Win, Is.EqualTo(false));
                Assert.That(updatedProperty.SpeakerHotkey.Ctrl, Is.EqualTo(true));
                Assert.That(updatedProperty.SpeakerHotkey.Shift, Is.EqualTo(false));
                Assert.That(updatedProperty.SpeakerHotkey.Alt, Is.EqualTo(true));
                Assert.That(updatedProperty.SpeakerHotkey.VirtualKey, Is.EqualTo("V"));
                Assert.That(updatedProperty.MicrophoneHotkeyEnabled, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkeyId, Is.EqualTo(9811));
                Assert.That(updatedProperty.MicrophoneHotkey.Win, Is.EqualTo(false));
                Assert.That(updatedProperty.MicrophoneHotkey.Ctrl, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkey.Shift, Is.EqualTo(false));
                Assert.That(updatedProperty.MicrophoneHotkey.Alt, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkey.VirtualKey, Is.EqualTo("N"));
                Assert.That(updatedProperty.TrayDoubleClickTarget, Is.EqualTo(TrayDoubleClickTarget.Speaker));
                Assert.That(updatedProperty.Startup, Is.EqualTo(false));
            });

            var Property2 = JsonSerializer.Deserialize<AppConfigProperty>(config113);
            Property2!.Theme = 0;
            var themeLight = AppJsonFormat.Update(Property2);
            Assert.That(themeLight.Theme, Is.EqualTo(SystemTheme.Light));

            var Property3 = JsonSerializer.Deserialize<AppConfigProperty>(config113);
            Property3!.Theme = (SystemTheme)1;
            var themeDark = AppJsonFormat.Update(Property3);
            Assert.That(themeDark.Theme, Is.EqualTo(SystemTheme.Dark));
        }

        [Test]
        public void UpdateFrom120()
        {
            var property = JsonSerializer.Deserialize<AppConfigProperty>(config120);
            var updatedProperty = AppJsonFormat.Update(property);

            Assert.Multiple(() =>
            {
                Assert.That(updatedProperty.Version, Is.EqualTo("1.3.0"));
                Assert.That(updatedProperty.SpeakerHotkeyEnabled, Is.EqualTo(true));
                Assert.That(updatedProperty.SpeakerHotkeyId, Is.EqualTo(9810));
                Assert.That(updatedProperty.SpeakerHotkey.VirtualKey, Is.EqualTo("V"));
                Assert.That(updatedProperty.MicrophoneHotkeyEnabled, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkeyId, Is.EqualTo(9811));
                Assert.That(updatedProperty.MicrophoneHotkey.Ctrl, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkey.Alt, Is.EqualTo(true));
                Assert.That(updatedProperty.MicrophoneHotkey.VirtualKey, Is.EqualTo("N"));
                Assert.That(updatedProperty.TrayDoubleClickTarget, Is.EqualTo(TrayDoubleClickTarget.Speaker));
                Assert.That(updatedProperty.LegacyHotkeyEnabled, Is.Null);
                Assert.That(updatedProperty.LegacyHotkeyId, Is.Null);
                Assert.That(updatedProperty.LegacyHotkey, Is.Null);
            });
        }

        [Test]
        public void CreateLatest()
        {
            var property = AppJsonFormat.CreateLatest();

            Assert.Multiple(() =>
            {
                Assert.That(property.Version, Is.EqualTo("1.3.0"));
                Assert.That(property.SpeakerHotkeyEnabled, Is.True);
                Assert.That(property.SpeakerHotkey.VirtualKey, Is.EqualTo("V"));
                Assert.That(property.MicrophoneHotkeyEnabled, Is.True);
                Assert.That(property.MicrophoneHotkey.Ctrl, Is.True);
                Assert.That(property.MicrophoneHotkey.Alt, Is.True);
                Assert.That(property.MicrophoneHotkey.VirtualKey, Is.EqualTo("N"));
                Assert.That(property.TrayDoubleClickTarget, Is.EqualTo(TrayDoubleClickTarget.Speaker));
            });
        }

        [Test]
        public void Keep130()
        {
            var property = JsonSerializer.Deserialize<AppConfigProperty>(config130);
            var updatedProperty = AppJsonFormat.Update(property);

            Assert.Multiple(() =>
            {
                Assert.That(updatedProperty.Version, Is.EqualTo("1.3.0"));
                Assert.That(updatedProperty.SpeakerHotkeyId, Is.EqualTo(9810));
                Assert.That(updatedProperty.MicrophoneHotkeyId, Is.EqualTo(9811));
                Assert.That(updatedProperty.TrayDoubleClickTarget, Is.EqualTo(TrayDoubleClickTarget.Speaker));
            });
        }
    }
}
