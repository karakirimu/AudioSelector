using Microsoft.VisualStudio.OLE.Interop;
using NativeCoreAudio;

namespace AudioTools
{
    public class VolumeChangeEvent : AudioEndpointVolumeCallback
    {
        private readonly SafeIAudioEndpointVolume audioEndpointVolume;
        private readonly SafeIMMDevice device;

        public VolumeChangeEvent(string deviceId) : base(deviceId)
        {
            using SafeIMMDeviceEnumerator enumerator = new();
            device = enumerator.GetDevice(deviceId);
            audioEndpointVolume = new SafeIAudioEndpointVolume(CLSCTX.CLSCTX_LOCAL_SERVER, device);
        }

        public void EnableNotification()
        {
            audioEndpointVolume.RegisterControlChangeNotify(this);
        }

        public void DisableNotification()
        {
            audioEndpointVolume.UnregisterControlChangeNotify(this);
        }
    }
}
