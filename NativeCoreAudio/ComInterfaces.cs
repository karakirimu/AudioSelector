using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;

namespace NativeCoreAudio
{
    /// <summary>
    /// ComInterfaces wrapper
    /// </summary>
    public static class ComInterfaces
    {
        // ObjBase.h
        public static readonly uint STGM_READ = 0x00000000;

        // devpkey.h
        public static readonly PROPERTYKEY PKEY_Device_FriendlyName
            = new()
            {
                fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0),
                pid = 14
            };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        public enum EDataFlow
        {
            eRender = 0,
            eCapture = eRender + 1,
            eAll = eCapture + 1,
            EDataFlow_enum_count = eAll + 1
        };

        public enum ERole
        {
            eConsole = 0,
            eMultimedia = eConsole + 1,
            eCommunications = eMultimedia + 1,
            ERoleEnumCount = eCommunications + 1
        };

        // mmdeviceapi.h
        public enum DeviceState : uint
        {
            ACTIVE = 0x00000001,
            DISABLED = 0x00000002,
            NOTPRESENT = 0x00000004,
            UNPLUGGED = 0x00000008,
            MASK_ALL = 0x0000000f
        };

        [ComImport]
        [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMNotificationClient
        {
            [PreserveSig]
            uint OnDeviceStateChanged(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId,
                /* [annotation][in] */
                DeviceState dwNewState);

            [PreserveSig]
            uint OnDeviceAdded(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);

            [PreserveSig]
            uint OnDeviceRemoved(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);

            [PreserveSig]
            uint OnDefaultDeviceChanged(
                /* [annotation][in] */
                in EDataFlow flow,
                /* [annotation][in] */
                in ERole role,
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);

            [PreserveSig]
            uint OnPropertyValueChanged(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId,
                /* [annotation][in] */
                PROPERTYKEY key);
        }

        [ComImport]
        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceCollection
        {
            [PreserveSig]
            uint GetCount(out uint pcDevices);

            [PreserveSig]
            uint Item(
                /* [annotation][in] */
                uint nDevice,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IMMDevice ppDevice);
        }

        [ComImport]
        [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            [PreserveSig]
            uint GetCount(out uint cProps);

            [PreserveSig]
            uint GetAt(uint iProp, out PROPERTYKEY pkey);

            [PreserveSig]
            uint GetValue(ref PROPERTYKEY key, out PROPVARIANT pv);

            [PreserveSig]
            uint SetValue(ref PROPERTYKEY key, PROPVARIANT pv);

            [PreserveSig]
            uint Commit();
        }

        [ComImport]
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDevice
        {
            //[PreserveSig]
            //uint Activate(
            //    /* [annotation][in] */
            //    [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            //    /* [annotation][in] */
            //    CLSCTX dwClsCtx,
            //    /* [annotation][unique][in] */
            //    PROPVARIANT pActivationParams,
            //    /* [annotation][iid_is][out] */
            //    [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            [PreserveSig]
            uint Activate(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
                /* [annotation][in] */
                CLSCTX dwClsCtx,
                /* [annotation][unique][in] */
                IntPtr pActivationParams,
                /* [annotation][iid_is][out] */
                [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            [PreserveSig]
            uint OpenPropertyStore(
                /* [annotation][in] */
                uint stgmAccess,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppProperties);

            [PreserveSig]
            uint GetId(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

            [PreserveSig]
            uint GetState(
                /* [annotation][out] */
                out uint pdwState);
        }

        public static readonly Guid CLSID_MMDeviceEnumerator
                = new("{BCDE0395-E52F-467C-8E3D-C4579291692E}");

        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceEnumerator
        {
            [PreserveSig]
            uint EnumAudioEndpoints(
                /* [annotation][in] */
                EDataFlow dataFlow,
                /* [annotation][in] */
                uint dwStateMask,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IMMDeviceCollection ppDevices);

            [PreserveSig]
            uint GetDefaultAudioEndpoint(
                /* [annotation][in] */
                EDataFlow dataFlow,
                /* [annotation][in] */
                ERole role,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IMMDevice ppEndpoint);

            [PreserveSig]
            uint GetDevice(
                /* [annotation][in] */
                string pwstrId,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IMMDevice ppDevice);

            [PreserveSig]
            uint RegisterEndpointNotificationCallback(
                [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient pClient);

            [PreserveSig]
            uint UnregisterEndpointNotificationCallback(
                [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient pClient);
        }

        /// <summary>
        /// Get LPWStr type strings
        /// </summary>
        /// <param name="prop">PROPVARIANT structure</param>
        /// <returns></returns>
        public static string GetPropVariantString(PROPVARIANT prop)
        {
            ComResult.Check(
                NativeMethods.PropVariantGetStringElem(prop, 0, out string name)
                );
            return name;
        }

        // -- devicetopology.h --
        public static readonly Guid IID_IAudioInputSelector
            = new("4F03DC02-5E6E-4653-8F72-A030C123D598");

        [ComImport]
        [Guid("4F03DC02-5E6E-4653-8F72-A030C123D598")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioInputSelector
        {
            [PreserveSig]
            uint GetSelection(
                /* [annotation][out] */
                out uint pnIdSelected);

            [PreserveSig]
            uint SetSelection(
                /* [annotation][in] */
                uint nIdSelect,
                /* [annotation][unique][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid pguidEventContext);

        };


        public static readonly Guid IID_IAudioOutputSelector
            = new("BB515F69-94A7-429e-8B9C-271B3F11A3AB");

        [ComImport]
        [Guid("BB515F69-94A7-429e-8B9C-271B3F11A3AB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioOutputSelector
        {
            [PreserveSig]
            uint GetSelection(
                /* [annotation][out] */
                out uint pnIdSelected);

            [PreserveSig]
            uint SetSelection(
                /* [annotation][in] */
                uint nIdSelect,
                /* [annotation][unique][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid pguidEventContext);

        };

        [ComImport]
        [Guid("6DAA848C-5EB0-45CC-AEA5-998A2CDA1FFB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPartsList
        {
            [PreserveSig]
            uint GetCount(
                /* [annotation][out] */
                out uint pCount);

            [PreserveSig]
            uint GetPart(
                /* [annotation][in] */
                uint nIndex,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IPart ppPart);

        };

        [ComImport]
        [Guid("45d37c3f-5140-444a-ae24-400789f3cbf3")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IControlInterface
        {
            [PreserveSig]
            uint GetName(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrName);

            [PreserveSig]
            uint GetIID(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPStruct)] out Guid pIID);

        };


        // devicetopology.h
        // In : Speakers, earphones, and so on ...
        // Out: some Microphones ...
        public enum DataFlow
        {
            In = 0,
            Out = (In + 1)
        };

        public enum PartType
        {
            Connector = 0,
            Subunit = (Connector + 1)
        };

        public enum ConnectorType
        {
            Unknown_Connector = 0,
            Physical_Internal = (Unknown_Connector + 1),
            Physical_External = (Physical_Internal + 1),
            Software_IO = (Physical_External + 1),
            Software_Fixed = (Software_IO + 1),
            Network = (Software_Fixed + 1)
        };

        [ComImport]
        [Guid("9c2c4058-23f5-41de-877a-df3af236a09e")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IConnector
        {
            [PreserveSig]
            uint GetType(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.U4)] out ConnectorType pType);

            [PreserveSig]
            uint GetDataFlow(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.U4)] out DataFlow pFlow);

            [PreserveSig]
            uint ConnectTo(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.Interface)] IConnector pConnectTo);

            [PreserveSig]
            uint Disconnect();

            [PreserveSig]
            uint IsConnected(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Bool)] out bool pbConnected);

            [PreserveSig]
            uint GetConnectedTo(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IConnector ppConTo);

            [PreserveSig]
            uint GetConnectorIdConnectedTo(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrConnectorId);

            [PreserveSig]
            uint GetDeviceIdConnectedTo(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);

        };

        [ComImport]
        [Guid("A09513ED-C709-4d21-BD7B-5F34C47F3947")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IControlChangeNotify
        {
            [PreserveSig]
            uint OnNotify(
                /* [annotation][in] */
                uint dwSenderProcessId,
                /* [annotation][unique][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid pguidEventContext);

        };

        [ComImport]
        [Guid("82149A85-DBA6-4487-86BB-EA8F7FEFCC71")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISubunit
        {
        };

        public static readonly Guid IID_IDeviceTopology
            = new("2A07407E-6497-4A18-9787-32F79BD0D98F");

        [ComImport]
        [Guid("2A07407E-6497-4A18-9787-32F79BD0D98F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDeviceTopology
        {
            [PreserveSig]
            uint GetConnectorCount(
            /* [annotation][out] */
            out uint pCount);

            [PreserveSig]
            uint GetConnector(
            /* [annotation][in] */
            uint nIndex,
            /* [annotation][out] */
            [MarshalAs(UnmanagedType.Interface)] out IConnector ppConnector);

            [PreserveSig]
            uint GetSubunitCount(
            /* [annotation][out] */
            out uint pCount);

            [PreserveSig]
            uint GetSubunit(
            /* [annotation][in] */
            uint nIndex,
            /* [annotation][out] */
            [MarshalAs(UnmanagedType.Interface)] out ISubunit ppSubunit);

            [PreserveSig]
            uint GetPartById(
            /* [annotation][in] */
            uint nId,
            /* [annotation][out] */
            [MarshalAs(UnmanagedType.Interface)] out IPart ppPart);

            [PreserveSig]
            uint GetDeviceId(
            /* [annotation][out] */
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);

            [PreserveSig]
            uint GetSignalPath(
            /* [annotation][in] */
            [MarshalAs(UnmanagedType.Interface)] IPart pIPartFrom,
            /* [annotation][in] */
            [MarshalAs(UnmanagedType.Interface)] IPart pIPartTo,
            /* [annotation][in] */
            [MarshalAs(UnmanagedType.Bool)] bool bRejectMixedPaths,
            /* [annotation][out] */
            [MarshalAs(UnmanagedType.Interface)] out IPartsList ppParts);

        };

        [ComImport]
        [Guid("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPart
        {
            [PreserveSig]
            uint GetName(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrName);

            [PreserveSig]
            uint GetLocalId(
                /* [annotation][out] */
                out uint pnId);

            [PreserveSig]
            uint GetGlobalId(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrGlobalId);

            [PreserveSig]
            uint GetPartType(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.U4)] out PartType pPartType);

            [PreserveSig]
            uint GetSubType(
                /* [out] */ out Guid pSubType);

            [PreserveSig]
            uint GetControlInterfaceCount(
                /* [annotation][out] */
                out uint pCount);

            [PreserveSig]
            uint GetControlInterface(
                /* [annotation][in] */
                uint nIndex,
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IControlInterface ppInterfaceDesc);

            [PreserveSig]
            uint EnumPartsIncoming(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IPartsList ppParts);

            [PreserveSig]
            uint EnumPartsOutgoing(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IPartsList ppParts);

            [PreserveSig]
            uint GetTopologyObject(
                /* [annotation][out] */
                [MarshalAs(UnmanagedType.Interface)] out IDeviceTopology ppTopology);

            [PreserveSig]
            uint Activate(
                /* [annotation][in] */
                CLSCTX dwClsContext,
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid refiid,
                /* [annotation][iid_is][out] */
                [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

            [PreserveSig]
            uint RegisterControlChangeCallback(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.Interface)] IControlChangeNotify pNotify);

            [PreserveSig]
            uint UnregisterControlChangeCallback(
                /* [annotation][in] */
                [MarshalAs(UnmanagedType.Interface)] IControlChangeNotify pNotify);

        };

        // System interface
        // https://docs.microsoft.com/en-us/answers/questions/669471/how-to-control-enable-audio-enhancements-with-code.html
        [ComImport]
        [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPolicyConfig
        {
            [PreserveSig]
            uint GetMixFormat();

            [PreserveSig]
            uint GetDeviceFormat();

            [PreserveSig]
            uint ResetDeviceFormat();

            [PreserveSig]
            uint SetDeviceFormat();

            [PreserveSig]
            uint GetProcessingPeriod();

            [PreserveSig]
            uint SetProcessingPeriod();

            [PreserveSig]
            uint GetShareMode();

            [PreserveSig]
            uint SetShareMode();

            [PreserveSig]
            uint GetPropertyValue(
                [MarshalAs(UnmanagedType.LPWStr)] string ppstrId,
                bool store,
                ref PROPERTYKEY key,
                out PROPVARIANT pv);

            [PreserveSig]
            uint SetPropertyValue(
                [MarshalAs(UnmanagedType.LPWStr)] string ppstrId,
                bool store,
                ref PROPERTYKEY key,
                PROPVARIANT pv);

            [PreserveSig]
            uint SetDefaultEndpoint(
                [MarshalAs(UnmanagedType.LPWStr)] string ppstrId,
                [MarshalAs(UnmanagedType.U4)] ERole eRole);

            [PreserveSig]
            uint SetDefaultEndpointForPolicy(
                [MarshalAs(UnmanagedType.LPWStr)] string ppstrId,
                [MarshalAs(UnmanagedType.U4)] EDataFlow eDataFlow);

            [PreserveSig]
            uint SetEndpointVisibility(
                [MarshalAs(UnmanagedType.LPWStr)] string ppstrId,
                bool visible);
        };
    }
}
