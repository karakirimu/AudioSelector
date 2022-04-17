using System;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IMMDeviceEnumerator
    /// </summary>
    public class SafeIMMDeviceEnumerator : SafeComObject
    {
        private bool disposedValue;

        private static object GetDeviceEnumerator()
        {
            Type MMDeviceEnumeratorType
                = Type.GetTypeFromCLSID(CLSID_MMDeviceEnumerator, true);

            object MMDeviceEnumerator
                = Activator.CreateInstance(MMDeviceEnumeratorType);

            return MMDeviceEnumerator;
        }

        public SafeIMMDeviceEnumerator()
        {
            SetReleaseObject(GetDeviceEnumerator());
        }

        public IMMDeviceEnumerator GetIMMDeviceEnumerator()
        {
            return GetReleaseObject() as IMMDeviceEnumerator;
        }

        public SafeIMMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role)
        {
            ComResult.Check(
            GetIMMDeviceEnumerator()
                .GetDefaultAudioEndpoint(dataFlow, role, out IMMDevice mMDevice)
                );

            return new SafeIMMDevice(mMDevice);
        }

        public SafeIMMDevice GetDevice(string endPoint)
        {
            ComResult.Check(
                GetIMMDeviceEnumerator().GetDevice(endPoint, out IMMDevice device)
            );
            return new SafeIMMDevice(device);
        }

        public void RegisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            ComResult.Check(GetIMMDeviceEnumerator().RegisterEndpointNotificationCallback(client));
        }

        public void UnregisterEndpointNotificationCallback(IMMNotificationClient client)
        {
            ComResult.Check(GetIMMDeviceEnumerator().UnregisterEndpointNotificationCallback(client));
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
