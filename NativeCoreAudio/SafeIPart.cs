using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IPart
    /// </summary>
    public class SafeIPart : SafeComObject
    {
        private bool disposedValue;

        /// <summary>
        /// QueryInterface type IPart derivation
        /// </summary>
        /// <param name="connectorFrom"></param>
        public SafeIPart(SafeIConnector connectorto)
        {
            Guid IID_IPart = new("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9");
            int hr = Marshal.QueryInterface(
                connectorto.GetIUnknownPointer(),
                ref IID_IPart,
                out IntPtr ppIPart);
            ComResult.Check(hr);

            SetReleaseObject(ppIPart);
        }

        /// <summary>
        /// IPartList::GetPart type IPart derivation
        /// </summary>
        /// <param name="index"></param>
        /// <param name="partsList"></param>
        public SafeIPart(uint index, IPartsList partsList)
        {
            ComResult.Check(partsList.GetPart(index, out IPart part));
            SetReleaseObject(part);
        }

        public IPartsList EnumPartsOutgoing()
        {
            ComResult.Check(
                GetIPart().EnumPartsOutgoing(out IPartsList partsList)
                );

            return partsList;
        }

        public PartType GetPartType()
        {
            ComResult.Check(
                GetIPart().GetPartType(out PartType partType)
                );
            return partType;
        }

        public object Activate(CLSCTX clsctx, Guid iid)
        {
            ComResult.Check(
                GetIPart().Activate(
                            clsctx,
                            iid,
                            out object output)
                );

            return output;
        }

        private IPart GetIPart()
        {
            return GetReleaseObject() as IPart;
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
