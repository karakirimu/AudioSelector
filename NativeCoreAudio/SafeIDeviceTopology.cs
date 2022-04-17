using Microsoft.VisualStudio.OLE.Interop;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IDeviceTopology
    /// </summary>
    public class SafeIDeviceTopology : SafeComObject
    {
        private bool disposedValue;

        public SafeIDeviceTopology(CLSCTX clsctx, SafeIMMDevice device)
        {
            object devicetopology
                = device.Activate(IID_IDeviceTopology, clsctx);
            SetReleaseObject(devicetopology);
        }

        public uint GetConnectorCount()
        {
            ComResult.Check(
                GetIDeviceTopology()
                .GetConnectorCount(out uint count)
                );
            return count;
        }

        public IConnector GetConnector(uint index)
        {
            ComResult.Check(
                GetIDeviceTopology().GetConnector(index, out IConnector connector)
                );
            return connector;
        }

        public uint GetSubunitCount()
        {
            ComResult.Check(
                GetIDeviceTopology()
                .GetSubunitCount(out uint count)
                );
            return count;
        }

        public ISubunit GetSubunit(uint index)
        {
            ComResult.Check(
                GetIDeviceTopology().GetSubunit(index, out ISubunit subunit)
                );
            return subunit;
        }

        private IDeviceTopology GetIDeviceTopology()
        {
            return GetReleaseObject() as IDeviceTopology;
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
