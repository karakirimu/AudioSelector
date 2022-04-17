using System;
using System.Runtime.InteropServices;

namespace NativeCoreAudio
{
    /// <summary>
    /// Used for automatic disposing of COM interface objects.
    /// base class for SafeXXX.
    /// </summary>
    public class SafeComObject : IDisposable
    {
        private bool disposedValue;
        private object releasePendingObject;

        public SafeComObject()
        {
            releasePendingObject = null;
        }

        public IntPtr GetIUnknownPointer()
        {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
            return Marshal.GetIUnknownForObject(releasePendingObject);
#pragma warning restore CA1416 // プラットフォームの互換性を検証
        }

        protected void SetReleaseObject(object needrelease)
        {
            releasePendingObject = needrelease;
        }

        protected void SetReleaseObject(IntPtr needrelease)
        {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
            releasePendingObject = Marshal.GetObjectForIUnknown(needrelease);
#pragma warning restore CA1416 // プラットフォームの互換性を検証
        }

        protected object GetReleaseObject()
        {
            return releasePendingObject;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
                    ComResult.Check(Marshal.ReleaseComObject(releasePendingObject));
#pragma warning restore CA1416 // プラットフォームの互換性を検証
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
