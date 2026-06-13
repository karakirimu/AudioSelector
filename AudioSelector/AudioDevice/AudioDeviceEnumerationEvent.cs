using AudioTools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static NativeCoreAudio.ComInterfaces;

namespace AudioSelector.AudioDevice
{
    /// <summary>
    /// This class manages audio devices connection list
    /// </summary>
    internal class AudioDeviceEnumerationEvent
    {
        private readonly AudioDeviceKind deviceKind;
        private NotificationEvent notificationEvent;
        public List<MultiMediaDevice> Devices { get; private set; }

        public delegate void DeviceEnumerationEvent(MultiMediaDevice device);
        public event DeviceEnumerationEvent Add;
        public event DeviceEnumerationEvent Remove;

        public AudioDeviceEnumerationEvent(AudioDeviceKind kind = AudioDeviceKind.Speaker)
        {
            deviceKind = kind;
            Devices = GetActiveDevices();
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

        private List<MultiMediaDevice> GetActiveDevices()
        {
            return deviceKind switch
            {
                AudioDeviceKind.Microphone => (List<MultiMediaDevice>)Enumeration.ListActiveCaptureDevices(),
                AudioDeviceKind.Speaker or _ => (List<MultiMediaDevice>)Enumeration.ListActiveRenderDevices(),
            };
        }

        /// <summary>
        /// Device changed event
        /// </summary>
        /// <param name="deviceId">Updated device id</param>
        /// <param name="state">Updated state</param>
        /// <returns>Always 0</returns>
        private uint OnDeviceStateChanged(string deviceId, DeviceState state)
        {
            Debug.WriteLine($"{deviceId}.{state}");

            List<MultiMediaDevice> latestDevices = GetActiveDevices();
            IReadOnlyCollection<string> latestIds = latestDevices.Select(device => device.Id).ToList();
            IReadOnlyCollection<string> currentIds = Devices.Select(device => device.Id).ToList();

            foreach (MultiMediaDevice removedDevice in Devices.Where(device => !latestIds.Contains(device.Id)).ToList())
            {
                Remove?.Invoke(removedDevice);
            }

            foreach (MultiMediaDevice addedDevice in latestDevices.Where(device => !currentIds.Contains(device.Id)))
            {
                Add?.Invoke(addedDevice);
            }

            Devices = latestDevices;
            return 0;
        }
    }
}
