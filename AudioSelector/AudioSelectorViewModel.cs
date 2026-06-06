using AudioSelector.AudioDevice;
using AudioSelector.Setting;
using AudioTools;
using System.Collections.ObjectModel;

namespace AudioSelector
{
    internal class AudioSelectorViewModel
    {
        public ObservableCollection<MultiMediaDevice> SpeakerDevices { get; set; }

        public ObservableCollection<MultiMediaDevice> MicrophoneDevices { get; set; }

        public AudioDeviceKind CurrentDeviceKind { get; set; }

        public IDeviceVolumeChangeEvent VolumeChangeEvent { get; set; }

        public IAppConfig AppConfig { get; set; }

    }
}
