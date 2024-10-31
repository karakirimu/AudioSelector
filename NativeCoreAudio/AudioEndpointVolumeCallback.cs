using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    public class AudioVolumeNotificationData
    {
        public string deviceId;
        public Guid eventContext;
        public bool muted;
        public float masterVolume;
        public float[] channelVolumes;
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class AudioEndpointVolumeCallback(string deviceId) : IAudioEndpointVolumeCallback
    {
        public delegate uint OnVolumeNotify(AudioVolumeNotificationData pNotify);
        public event OnVolumeNotify ValueChanged;
        private readonly string deviceId = deviceId;

        public uint OnNotify(IntPtr pNotify)
        {
            Debug.WriteLine("[AudioEndpointVolumeCallback.OnNotify] ValueChanged");
            var o = Marshal.PtrToStructure<AUDIO_VOLUME_NOTIFICATION_DATA>(pNotify);

            AudioVolumeNotificationData args = new()
            {
                deviceId = deviceId,
                eventContext = new Guid(
                (int)o.guidEventContext.Data1,
                (short)o.guidEventContext.Data2,
                (short)o.guidEventContext.Data3,
                o.guidEventContext.Data4
                ),
                muted = o.bMuted,
                masterVolume = o.fMasterVolume,
                channelVolumes = new float[o.nChannels]
            };
            Array.Copy(o.afChannelVolumes, args.channelVolumes, o.nChannels);

            return ValueChanged?.Invoke(args) ?? 0;
        }
    }
}
