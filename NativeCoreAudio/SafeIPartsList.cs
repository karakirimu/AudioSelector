using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IPartsList
    /// </summary>
    public class SafeIPartsList : SafeComObject
    {
        private bool disposedValue;

        public SafeIPartsList(SafeIPart part)
        {
            SetReleaseObject(part.EnumPartsOutgoing());
        }

        public uint GetCount()
        {
            ComResult.Check(
                GetIPartsList().GetCount(out uint part)
                );
            return part;
        }

        public SafeIPart GetPart(uint index)
        {
            return new SafeIPart(index, GetIPartsList());
        }

        private IPartsList GetIPartsList()
        {
            return GetReleaseObject() as IPartsList;
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
