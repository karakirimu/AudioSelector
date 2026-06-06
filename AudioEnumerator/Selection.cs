using NativeCoreAudio;
using System;
using static NativeCoreAudio.ComInterfaces;

namespace AudioTools
{
    public class Selection
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
            Select(speakerId);
        }

        /// <summary>
        /// Set system audio device
        /// </summary>
        /// <param name="deviceId">audio device id</param>
        public static void Select(string deviceId)
        {
            using SafeIPolicyConfig config = new();

            try
            {
                config.SetDefaultEndpoint(deviceId, ERole.eConsole);
                config.SetDefaultEndpoint(deviceId, ERole.eMultimedia);
                config.SetDefaultEndpoint(deviceId, ERole.eCommunications);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
