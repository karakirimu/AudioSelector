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
                Assert.That(updatedProperty.Version, Is.EqualTo("1.0.0"));
                Assert.That(updatedProperty.Theme, Is.EqualTo(SystemTheme.System));
                Assert.That(updatedProperty.Language, Is.EqualTo("System"));
                Assert.That(updatedProperty.Hotkey_enabled, Is.EqualTo(true));
                Assert.That(updatedProperty.Hotkey_id, Is.EqualTo(9810));
                Assert.That(updatedProperty.Hotkey.Win, Is.EqualTo(false));
                Assert.That(updatedProperty.Hotkey.Ctrl, Is.EqualTo(true));
                Assert.That(updatedProperty.Hotkey.Shift, Is.EqualTo(false));
                Assert.That(updatedProperty.Hotkey.Alt, Is.EqualTo(true));
                Assert.That(updatedProperty.Hotkey.VirtualKey, Is.EqualTo("V"));
                Assert.That(updatedProperty.Startup, Is.EqualTo(false));
            });

            var Property2 = JsonSerializer.Deserialize<AppConfigProperty>(config113);
            Property2.Theme = 0;
            var themeLight = AppJsonFormat.Update(Property2);
            Assert.That(themeLight.Theme, Is.EqualTo(SystemTheme.Light));

            var Property3 = JsonSerializer.Deserialize<AppConfigProperty>(config113);
            Property3.Theme = (SystemTheme)1;
            var themeDark = AppJsonFormat.Update(Property3);
            Assert.That(themeDark.Theme, Is.EqualTo(SystemTheme.Dark));
        }
    }
}