using System.Runtime.InteropServices;

namespace NativeCoreAudio
{
    /// <summary>
    /// Exception output for COM-related errors
    /// </summary>
    public static class ComResult
    {
        public static void Check(int result)
        {
            if (result != 0)
            {
                throw new COMException();
            }
        }

        public static void Check(uint result)
        {
            if (result != 0)
            {
                throw new COMException();
            }
        }
    }
}
