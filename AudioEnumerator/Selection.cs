using NativeCoreAudio;
using System;
using static NativeCoreAudio.ComInterfaces;

namespace AudioTools
{
    class Selection
    {
        public Selection()
        {

        }

        /// <summary>
        /// Set system audio outputdevice
        /// </summary>
        /// <param name="speakerId">audio device id</param>
        public static void SelectOutput(string speakerId)
        {
            using SafeIPolicyConfig config = new();

            try
            {
                config.SetDefaultEndpoint(speakerId, ERole.eConsole);
                config.SetDefaultEndpoint(speakerId, ERole.eMultimedia);
                config.SetDefaultEndpoint(speakerId, ERole.eCommunications);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
