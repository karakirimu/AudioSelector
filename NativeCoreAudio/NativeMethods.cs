using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;

namespace NativeCoreAudio
{
    static class NativeMethods
    {
        [Flags]
        public enum CLSCTX : uint
        {
            INPROC_SERVER = 0x1,
            INPROC_HANDLER = 0x2,
            LOCAL_SERVER = 0x4,
            INPROC_SERVER16 = 0x8,
            REMOTE_SERVER = 0x10,
            INPROC_HANDLER16 = 0x20,
            RESERVED1 = 0x40,
            RESERVED2 = 0x80,
            RESERVED3 = 0x100,
            RESERVED4 = 0x200,
            NO_CODE_DOWNLOAD = 0x400,
            RESERVED5 = 0x800,
            NO_CUSTOM_MARSHAL = 0x1000,
            ENABLE_CODE_DOWNLOAD = 0x2000,
            NO_FAILURE_LOG = 0x4000,
            DISABLE_AAA = 0x8000,
            ENABLE_AAA = 0x10000,
            FROM_DEFAULT_CONTEXT = 0x20000,
            ACTIVATE_32_BIT_SERVER = 0x40000,
            ACTIVATE_64_BIT_SERVER = 0x80000,
            INPROC = INPROC_SERVER | INPROC_HANDLER,
            SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
            ALL = SERVER | INPROC_HANDLER
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MULTI_QI
        {
            [MarshalAs(UnmanagedType.LPStruct)] public Guid pIID;
            [MarshalAs(UnmanagedType.Interface)] public object pItf;
            public int hr;
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        public static extern void CoCreateInstanceEx(
            [MarshalAs(UnmanagedType.LPStruct)] in Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            CLSCTX dwClsCtx,
            IntPtr pServerInfo,
            uint cmq,
            ref MULTI_QI[] pResults);

        [DllImport("Propsys.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern uint PropVariantGetStringElem(
          PROPVARIANT propvar,
          uint iElem,
          [MarshalAs(UnmanagedType.LPWStr)] out string psz
        );

    }
}
