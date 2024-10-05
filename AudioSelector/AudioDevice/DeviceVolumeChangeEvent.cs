using AudioTools;
using NativeCoreAudio;
using System.Collections.Generic;

namespace AudioSelector.AudioDevice
{
    public interface IDeviceVolumeChangeEvent
    {
        public delegate uint DeviceEnumerationEvent(AudioVolumeNotificationData args);
        public event DeviceEnumerationEvent Update;
    }

    internal class DeviceVolumeChangeEvent : IDeviceVolumeChangeEvent
    {
        private readonly Dictionary<string, VolumeChangeEvent> volumeChangeEvents;
        public event IDeviceVolumeChangeEvent.DeviceEnumerationEvent Update;

        public DeviceVolumeChangeEvent()
        { 
            volumeChangeEvents = [];
        }


        public void AddCallback(string deviceId)
        {
            VolumeChangeEvent volumeChangeEvent = new(deviceId);
            volumeChangeEvent.ValueChanged += OnValueChanged;
            volumeChangeEvent.EnableNotification();
            volumeChangeEvents.Add(deviceId, volumeChangeEvent);
        }

        public void RemoveCallback(string deviceId)
        {
            if (volumeChangeEvents.TryGetValue(deviceId, out VolumeChangeEvent value))
            {
                value.DisableNotification();
                value.ValueChanged -= OnValueChanged;
                volumeChangeEvents.Remove(deviceId);
            }
        }

        private uint OnValueChanged(AudioVolumeNotificationData notify)
        {
            return Update?.Invoke(notify) ?? 0;
        }
    }
}
