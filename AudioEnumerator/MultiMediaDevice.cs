using System.Collections.Generic;
using static NativeCoreAudio.ComInterfaces;

namespace AudioTools
{
    /// <summary>
    /// 
    /// </summary>
    public class Connector
    {
        public bool Connected { get; internal set; }
        public DataFlow Flow { get; internal set; }
        public ConnectorType Type { get; internal set; }
        public uint InputSelectionId { get; internal set; }
    }

    /// <summary>
    /// It contains generic audio device data
    /// </summary>
    public class MultiMediaDevice
    {
        public IReadOnlyCollection<Connector> Connectors { get; internal set; }
        public string DeviceName { get; internal set; }
        public string Id { get; internal set; }
    }
}
