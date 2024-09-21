using AudioTools;
using System.Collections.Generic;
using System.Diagnostics;
using static NativeCoreAudio.ComInterfaces;

namespace AudioSelector.AudioDevice
{
    /// <summary>
    /// This class manages audio devices connection list
    /// </summary>
    internal class AudioDeviceEnumerationEvent
    {
        private NotificationEvent notificationEvent;
        public List<MultiMediaDevice> Devices { get; private set; }

        public delegate void DeviceEnumerationEvent(MultiMediaDevice device);
        public event DeviceEnumerationEvent Add;
        public event DeviceEnumerationEvent Remove;

        public AudioDeviceEnumerationEvent()
        {
            Devices = (List<MultiMediaDevice>)Enumeration.ListActiveRenderDevices();
        }

        /// <summary>
        /// It starts audio devices notification event
        /// </summary>
        public void Start()
        {
            notificationEvent = new();
            notificationEvent.DeviceStateChanged += OnDeviceStateChanged;
            notificationEvent.EnableNotification();
        }

        /// <summary>
        /// It stops audio devices notification event
        /// </summary>
        public void Stop()
        {
            notificationEvent.DeviceStateChanged -= OnDeviceStateChanged;
            notificationEvent.DisableNotification();
        }

        /// <summary>
        /// This function removes specific id from audio devices list
        /// </summary>
        /// <param name="removedeviceid">Audio endpoint id for removing</param>
        /// <returns></returns>
        private bool RemoveFromId(string removedeviceid)
        {
            foreach (MultiMediaDevice device in Devices)
            {
                if (device.Id == removedeviceid)
                {
                    return Devices.Remove(device);
                }
            }

            return false;
        }

        /// <summary>
        /// Device changed event
        /// </summary>
        /// <param name="deviceId">Updated device id</param>
        /// <param name="state">Updated state</param>
        /// <returns>Always 0</returns>
        private uint OnDeviceStateChanged(string deviceId, DeviceState state)
        {
            Trace.WriteLine($"{deviceId}.{state}");

            switch (state)
            {
                case DeviceState.ACTIVE:
                    MultiMediaDevice multiMedia = Enumeration.GetInformationFromId(deviceId);
                    Devices.Add(multiMedia);
                    Add?.Invoke(multiMedia);
                    break;
                case DeviceState.DISABLED:
                case DeviceState.UNPLUGGED:
                case DeviceState.NOTPRESENT:
                    if (RemoveFromId(deviceId))
                    {
                        Remove?.Invoke(Enumeration.GetInformationFromId(deviceId));
                    }
                    break;
                case DeviceState.MASK_ALL:
                    break;
                default:
                    break;
            }

            return 0;
        }
    }
}
