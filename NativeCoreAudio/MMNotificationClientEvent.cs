using System.Diagnostics;
using System.Runtime.InteropServices;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Audio device notification event wrapper
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class MMNotificationClientEvent : IMMNotificationClient
    {
        public delegate uint NotificationEvent<T>(T args);
        public event NotificationEvent<string> Add;
        public event NotificationEvent<string> Remove;

        public delegate uint NotificationEvent<T, U>(T arg1, U arg2);
        public event NotificationEvent<string, DeviceState> DeviceStateChanged;
        public event NotificationEvent<string, PROPERTYKEY> PropertyValueChanged;

        public delegate uint NotificationEvent<T, U, V>(T arg1, U arg2, V arg3);
        public event NotificationEvent<EDataFlow, ERole, string> DefaultDeviceChanged;


        public uint OnDeviceStateChanged(
            [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId,
            DeviceState dwNewState)
        {
            Debug.WriteLine("DeviceStateChanged");
            return DeviceStateChanged?.Invoke(pwstrDeviceId, dwNewState) ?? 0;
        }

        public uint OnDeviceAdded(
            [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId)
        {
            Debug.WriteLine("Added");
            return Add?.Invoke(pwstrDeviceId) ?? 0;
        }

        public uint OnDeviceRemoved(
            [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId)
        {
            Debug.WriteLine("Removed");
            return Remove?.Invoke(pwstrDeviceId) ?? 0;
        }

        public uint OnDefaultDeviceChanged(
            in EDataFlow flow,
            in ERole role,
            [MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId)
        {
            Debug.WriteLine("[MMNotificationClientEvent.DefaultDeviceChanged] DefaultDeviceChanged");
            return DefaultDeviceChanged?.Invoke(flow, role, pwstrDefaultDeviceId) ?? 0;
        }

        public uint OnPropertyValueChanged(
            [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId,
            PROPERTYKEY key)
        {
            Debug.WriteLine($"[MMNotificationClientEvent.PropertyValueChanged] {pwstrDeviceId} key :{key.fmtid} pid: {key.pid}");
            return PropertyValueChanged?.Invoke(pwstrDeviceId, key) ?? 0;
        }
    }
}
