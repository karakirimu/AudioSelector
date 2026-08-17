using AudioTools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
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
        private Dispatcher notificationDispatcher;
        private bool isStarted;
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
            notificationDispatcher = Dispatcher.CurrentDispatcher;
            notificationEvent = new();
            notificationEvent.Add += OnDeviceAdded;
            notificationEvent.Remove += OnDeviceRemoved;
            notificationEvent.DeviceStateChanged += OnDeviceStateChanged;
            isStarted = true;
            notificationEvent.EnableNotification();
            RefreshDevices();
        }

        /// <summary>
        /// It stops audio devices notification event
        /// </summary>
        public void Stop()
        {
            notificationEvent.DisableNotification();
            isStarted = false;
            notificationEvent.Add -= OnDeviceAdded;
            notificationEvent.Remove -= OnDeviceRemoved;
            notificationEvent.DeviceStateChanged -= OnDeviceStateChanged;
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
        /// Device added event
        /// </summary>
        /// <param name="deviceId">Added device id</param>
        /// <returns>Always 0</returns>
        private uint OnDeviceAdded(string deviceId)
        {
            return QueueDeviceRefresh(deviceId, "Added");
        }

        /// <summary>
        /// Device removed event
        /// </summary>
        /// <param name="deviceId">Removed device id</param>
        /// <returns>Always 0</returns>
        private uint OnDeviceRemoved(string deviceId)
        {
            return QueueDeviceRefresh(deviceId, "Removed");
        }

        /// <summary>
        /// Device state changed event
        /// </summary>
        /// <param name="deviceId">Updated device id</param>
        /// <param name="state">Updated state</param>
        /// <returns>Always 0</returns>
        private uint OnDeviceStateChanged(string deviceId, DeviceState state)
        {
            return QueueDeviceRefresh(deviceId, state.ToString());
        }

        private uint QueueDeviceRefresh(string deviceId, string notification)
        {
            Debug.WriteLine($"{deviceId}.{notification}");
            _ = notificationDispatcher?.BeginInvoke(RefreshDevices, DispatcherPriority.DataBind);
            return 0;
        }

        private void RefreshDevices()
        {
            if (!isStarted)
            {
                return;
            }

            List<MultiMediaDevice> latestDevices = GetActiveDevices();
            IReadOnlyCollection<string> latestIds = latestDevices.Select(device => device.Id).ToList();
            IReadOnlyCollection<string> currentIds = Devices.Select(device => device.Id).ToList();
            List<MultiMediaDevice> removedDevices = Devices.Where(device => !latestIds.Contains(device.Id)).ToList();
            List<MultiMediaDevice> addedDevices = latestDevices.Where(device => !currentIds.Contains(device.Id)).ToList();

            Devices = latestDevices;

            foreach (MultiMediaDevice removedDevice in removedDevices)
            {
                Remove?.Invoke(removedDevice);
            }

            foreach (MultiMediaDevice addedDevice in addedDevices)
            {
                Add?.Invoke(addedDevice);
            }
        }
    }
}
