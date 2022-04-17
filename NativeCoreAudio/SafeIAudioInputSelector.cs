using Microsoft.VisualStudio.OLE.Interop;
using System;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IAudioInputSelector
    /// </summary>
    public class SafeIAudioInputSelector : SafeComObject
    {
        private bool disposedValue;

        public SafeIAudioInputSelector(CLSCTX clsctx, SafeIPart part)
        {
            SetReleaseObject(part.Activate(clsctx, IID_IAudioInputSelector));
        }

        public uint GetSelection()
        {
            ComResult.Check(
                GetIAudioInputSelector().GetSelection(out uint selectedid)
                );
            return selectedid;
        }

        public void SetSelection(uint selectedId, Guid eventContext)
        {
            ComResult.Check(
                GetIAudioInputSelector().SetSelection(selectedId, eventContext)
                );
        }

        private IAudioInputSelector GetIAudioInputSelector()
        {
            return GetReleaseObject() as IAudioInputSelector;
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
