using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IConnector
    /// </summary>
    public class SafeIConnector : SafeComObject
    {
        private bool disposedValue;

        public SafeIConnector(uint index, SafeIDeviceTopology topology)
        {
            SetReleaseObject(topology.GetConnector(index));
        }

        SafeIConnector(IConnector connected)
        {
            SetReleaseObject(connected);
        }

        public bool IsConnected()
        {
            ComResult.Check(
                GetIConnector().IsConnected(out bool connected)
                );
            return connected;
        }

        public SafeIConnector GetConnectedTo()
        {
            ComResult.Check(
                GetIConnector().GetConnectedTo(out IConnector connectedto)
                );

            return new SafeIConnector(connectedto);
        }

        public new ConnectorType GetType()
        {
            ComResult.Check(
                GetIConnector().GetType(out ConnectorType type)
                );
            return type;
        }

        public DataFlow GetDataFlow()
        {
            ComResult.Check(
                GetIConnector().GetDataFlow(out DataFlow dataflow)
                );
            return dataflow;
        }

        private IConnector GetIConnector()
        {
            return GetReleaseObject() as IConnector;
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
