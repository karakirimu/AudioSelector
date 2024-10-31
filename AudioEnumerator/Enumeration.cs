using Microsoft.VisualStudio.OLE.Interop;
using NativeCoreAudio;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static NativeCoreAudio.ComInterfaces;

namespace AudioTools
{
    /// <summary>
    /// Audio device enumerator
    /// </summary>
    public class Enumeration
    {
        public Enumeration()
        {

        }

        /// <summary>
        /// Get PKEY_Device_FriendlyName
        /// </summary>
        /// <param name="device">NativeCoreAudio wrappered IMMDevice</param>
        /// <returns>PKEY_Device_FriendlyName</returns>
        private static string GetFriendlyName(SafeIMMDevice device)
        {
            using SafeIPropertyStore propertyStore = new(STGM_READ, device);
            PROPVARIANT prop = propertyStore.GetValue(PKEY_Device_FriendlyName);
            return GetPropVariantString(prop);
        }

        /// <summary>
        /// Get eRender AudioDeviceEndpoint
        /// </summary>
        /// <param name="role">Audio role</param>
        /// <returns>eRender audio device Id</returns>
        public static string GetDefaultDeviceEndpointId(ERole role)
        {
            using SafeIMMDeviceEnumerator enumerator = new();
            using SafeIMMDevice device
                = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, role);

            return device.GetId();
        }

        /// <summary>
        /// Get eRender active devices list
        /// </summary>
        /// <returns>Device information list</returns>
        public static IReadOnlyCollection<MultiMediaDevice> ListActiveRenderDevices()
        {
            List<MultiMediaDevice> multiMediaDevices = [];
            using SafeIMMDeviceEnumerator enumerator = new();
            using SafeIMMDeviceCollection collection
                = new(EDataFlow.eRender, DeviceState.ACTIVE, enumerator);

            uint deviceCount = collection.GetCount();

            for (uint i = 0; i < deviceCount; i++)
            {
                MultiMediaDevice multiMedia = new();
                using SafeIMMDevice device = new(i, collection);

                try
                {
                    multiMedia.Connectors = ListConnector(device);
                    multiMedia.Id = device.GetId();
                    multiMedia.DeviceName = GetFriendlyName(device);
                    multiMediaDevices.Add(multiMedia);

                }catch(COMException ex)
                {
                    Debug.WriteLine($"[Enumeration.ListActiveRenderDevices] {ex.Message}");
                }

            }

            return multiMediaDevices;
        }

        /// <summary>
        /// Get specific device attributes 
        /// </summary>
        /// <param name="deviceId">Endpoint id that you wants to get</param>
        /// <returns>MultiMediaDevice data class</returns>
        public static MultiMediaDevice GetInformationFromId(string deviceId)
        {
            using SafeIMMDeviceEnumerator enumerator = new();
            using SafeIMMDevice device = enumerator.GetDevice(deviceId);
            MultiMediaDevice multiMedia = new()
            {
                Connectors = ListConnector(device),
                Id = deviceId,
                DeviceName = GetFriendlyName(device)
            };

            return multiMedia;
        }

        /// <summary>
        /// Get master volume level from device
        /// </summary>
        /// <param name="deviceId">Specific device id</param>
        /// <returns>volume level (0.0 ~ 1.0)</returns>
        public static float GetMasterVolume(string deviceId)
        {
            using SafeIMMDeviceEnumerator enumerator = new();
            using SafeIMMDevice device = enumerator.GetDevice(deviceId);
            using SafeIAudioEndpointVolume volume = new(CLSCTX.CLSCTX_LOCAL_SERVER, device);
            return volume.GetMasterVolumeLevelScalar();
        }

        public static bool GetMute(string deviceId)
        {
            using SafeIMMDeviceEnumerator enumerator = new();
            using SafeIMMDevice device = enumerator.GetDevice(deviceId);
            using SafeIAudioEndpointVolume volume = new(CLSCTX.CLSCTX_LOCAL_SERVER, device);
            return volume.GetMute();
        }

        /// <summary>
        /// List local audio device
        /// </summary>
        /// <param name="device">NativeCoreAudio wrappered IMMDevice</param>
        /// <returns>Device information list</returns>
        public static IReadOnlyCollection<Connector> ListConnector(SafeIMMDevice device)
        {
            List<Connector> connectors = [];

            using SafeIDeviceTopology deviceTopology
                = new(CLSCTX.CLSCTX_LOCAL_SERVER, device);

            uint connectorcount = deviceTopology.GetConnectorCount();

            for (uint i = 0; i < connectorcount; i++)
            {
                using SafeIConnector connector = new(i, deviceTopology);
                Connector result = new();

                result.Connected = connector.IsConnected();
                result.Flow = connector.GetDataFlow();
                result.Type = connector.GetType();

                connectors.Add(result);
            }

            return connectors;
        }
    }
}
