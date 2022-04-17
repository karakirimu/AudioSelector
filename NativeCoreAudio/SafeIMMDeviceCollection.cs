using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IMMDeviceCollection
    /// </summary>
    public class SafeIMMDeviceCollection : SafeComObject
    {
        private bool disposedValue;

        public SafeIMMDeviceCollection(EDataFlow eDataFlow,
            DeviceState deviceState,
            SafeIMMDeviceEnumerator deviceEnumerator)
        {
            uint hr = deviceEnumerator.GetIMMDeviceEnumerator().EnumAudioEndpoints(
                eDataFlow,
                (uint)deviceState,
                out IMMDeviceCollection deviceCollection);

            ComResult.Check(hr);
            SetReleaseObject(deviceCollection);
        }

        public uint GetCount()
        {
            ComResult.Check(
                GetIMMDeviceCollection().GetCount(out uint devices)
                );
            return devices;
        }

        public IMMDevice Item(uint index)
        {
            ComResult.Check(
                GetIMMDeviceCollection().Item(index, out IMMDevice device)
                );
            return device;
        }

        private IMMDeviceCollection GetIMMDeviceCollection()
        {
            return GetReleaseObject() as IMMDeviceCollection;
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
