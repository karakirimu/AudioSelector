using Microsoft.VisualStudio.OLE.Interop;
using System;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IAudioOutputSelector
    /// </summary>
    public class SafeIAudioOutputSelector : SafeComObject
    {
        private bool disposedValue;

        public SafeIAudioOutputSelector(CLSCTX clsctx, SafeIPart part)
        {
            SetReleaseObject(part.Activate(clsctx, IID_IAudioOutputSelector));
        }

        public uint GetSelection()
        {
            ComResult.Check(
                GetIAudioOutputSelector().GetSelection(out uint selectedid)
                );
            return selectedid;
        }

        public void SetSelection(uint selectedId, Guid eventContext)
        {
            ComResult.Check(
                GetIAudioOutputSelector().SetSelection(selectedId, eventContext)
                );
        }

        private IAudioOutputSelector GetIAudioOutputSelector()
        {
            return GetReleaseObject() as IAudioOutputSelector;
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
