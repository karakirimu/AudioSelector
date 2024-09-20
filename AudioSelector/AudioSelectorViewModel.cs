using AudioSelector.Setting;
using AudioTools;
using System.Collections.ObjectModel;

namespace AudioSelector
{
    internal class AudioSelectorViewModel
    {
        public ObservableCollection<MultiMediaDevice> Devices { get; set; }

        public IAppConfig AppConfig { get; set; }

    }
}
