using NativeCoreAudio;

namespace AudioTools
{
    public class NotificationEvent : MMNotificationClientEvent
    {
        private readonly SafeIMMDeviceEnumerator deviceEnumerator;

        public NotificationEvent()
        {
            deviceEnumerator = new();
        }

        /// <summary>
        /// Start audio device notification (e.g. add, remove, ...) event.
        /// After use, DisableNotification must be called.
        /// </summary>
        public void EnableNotification()
        {
            deviceEnumerator.RegisterEndpointNotificationCallback(this);
        }

        /// <summary>
        /// Stop audio device notification event.
        /// </summary>
        public void DisableNotification()
        {
            deviceEnumerator.UnregisterEndpointNotificationCallback(this);
        }
    }
}
