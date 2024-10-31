using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    public class VolumeStepInfo
    {
        public uint Step { get; set; }
        public uint StepCount { get; set; }
    }

    public class VoluemRange
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float Increment { get; set; }
    }

    public class SafeIAudioEndpointVolume : SafeComObject
    {
        private bool disposedValue;

        public SafeIAudioEndpointVolume(CLSCTX clsctx, SafeIMMDevice device)
        {
            object audioendpointvolume
                = device.Activate(IID_IAudioEndpointVolume, clsctx);
            SetReleaseObject(audioendpointvolume);
        }

        public void RegisterControlChangeNotify(IAudioEndpointVolumeCallback callback)
        {
            ComResult.Check(GetIDeviceEndpointVolume()
                .RegisterControlChangeNotify(callback)
            );
        }

        public void UnregisterControlChangeNotify(IAudioEndpointVolumeCallback callback)
        {
            ComResult.Check(GetIDeviceEndpointVolume()
                .UnregisterControlChangeNotify(callback)
            );
        }

        public uint GetChannelCount()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetChannelCount(out uint count));
            return count;
        }

        public void SetMasterVolumeLevel(float levelDb, Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().SetMasterVolumeLevel(levelDb, eventContext));
        }

        public void SetMasterVolumeLevelScalar(float level, Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().SetMasterVolumeLevelScalar(level, eventContext));
        }

        public float GetMasterVolumeLevel()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetMasterVolumeLevel(out float levelDb));
            return levelDb;
        }

        public float GetMasterVolumeLevelScalar()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetMasterVolumeLevelScalar(out float level));
            return level;
        }

        public void SetChannelVolumeLevel(uint channel, float levelDb, Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().SetChannelVolumeLevel(channel, levelDb, eventContext));
        }
        public void SetChannelVolumeLevelScalar(uint channel, float level, Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().SetChannelVolumeLevelScalar(channel, level, eventContext));
        }

        public float GetChannelVolumeLevel(uint channel)
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetChannelVolumeLevel(channel, out float levelDb));
            return levelDb;
        }

        public float GetChannelVolumeLevelScalar(uint channel)
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetChannelVolumeLevelScalar(channel, out float level));
            return level;
        }

        public void SetMute(bool mute, Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().SetMute(mute, eventContext));
        }

        public bool GetMute()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetMute(out bool mute));
            return mute;
        }

        public VolumeStepInfo GetVolumeStepInfo()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetVolumeStepInfo(out uint step, out uint stepCount));
            return new VolumeStepInfo() { Step = step, StepCount = stepCount };
        }

        public void VolumeStepUp(Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().VolumeStepUp(eventContext));
        }

        public void VolumeStepDown(Guid eventContext)
        {
            ComResult.Check(GetIDeviceEndpointVolume().VolumeStepDown(eventContext));
        }

        public short QueryHardwareSupport()
        {
            ComResult.Check(GetIDeviceEndpointVolume().QueryHardwareSupport(out short support));
            return support;
        }

        public VoluemRange GetVolumeRange()
        {
            ComResult.Check(GetIDeviceEndpointVolume().GetVolumeRange(out float min, out float max, out float increment));
            return new VoluemRange() { Min = min, Max = max, Increment = increment };
        }

        private IAudioEndpointVolume GetIDeviceEndpointVolume()
        {
            return GetReleaseObject() as IAudioEndpointVolume;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    base.Dispose(false);
                }
                disposedValue = true;
            }
        }
    }
}
