using Microsoft.VisualStudio.OLE.Interop;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IPropertyStore
    /// </summary>
    public class SafeIPropertyStore : SafeComObject
    {
        private bool disposedValue;

        /// <summary>
        /// IMMDevice::OpenPropertyStore
        /// </summary>
        /// <param name="stgm"></param>
        /// <param name="device"></param>
        public SafeIPropertyStore(uint stgm, SafeIMMDevice device)
        {
            SetReleaseObject(device.OpenPropertyStore(stgm));
        }

        public PROPVARIANT GetValue(PROPERTYKEY key)
        {
            ComResult.Check(
                GetIPropertyStore()
                .GetValue(ref key, out PROPVARIANT prop));
            return prop;
        }

        public IPropertyStore GetIPropertyStore()
        {
            return GetReleaseObject() as IPropertyStore;
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
