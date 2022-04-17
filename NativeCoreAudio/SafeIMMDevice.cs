using Microsoft.VisualStudio.OLE.Interop;
using System;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IMMDevice
    /// </summary>
    public class SafeIMMDevice : SafeComObject
    {
        private bool disposedValue;

        /// <summary>
        /// Auto disposable IMMDevice.
        /// This constructor based on IDeviceCollection::Item.
        /// </summary>
        /// <param name="index">Device index</param>
        /// <param name="deviceCollection">Auto disposable IDeviceCollection handler</param>
        public SafeIMMDevice(uint index,
            SafeIMMDeviceCollection deviceCollection)
        {
            SetReleaseObject(deviceCollection.Item(index));
        }

        public SafeIMMDevice(IMMDevice device)
        {
            SetReleaseObject(device);
        }

        private IMMDevice GetIMMDevice()
        {
            return GetReleaseObject() as IMMDevice;
        }

        public IPropertyStore OpenPropertyStore(uint stgm)
        {
            ComResult.Check(
                GetIMMDevice().OpenPropertyStore(stgm, out IPropertyStore propertyStore)
                );
            return propertyStore;
        }

        public string GetId()
        {
            uint hr = GetIMMDevice().GetId(out string id);
            ComResult.Check(hr);
            return id;
        }

        public object Activate(Guid iid,
            CLSCTX clsctx)
        {
            uint hr = GetIMMDevice().Activate(iid,
            clsctx,
            IntPtr.Zero,
            out object iidobject);
            ComResult.Check(hr);

            return iidobject;
        }

        public object Activate(Guid iid,
            CLSCTX clsctx,
            IntPtr activationParams)
        {
            uint hr = GetIMMDevice().Activate(iid,
            clsctx,
            activationParams,
            out object iidobject);
            ComResult.Check(hr);

            return iidobject;
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
