using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AudioSelector
{
    internal static class StoreCrashReporting
    {
        private static bool registered;

        public static void Register(Application app)
        {
            if (registered)
            {
                return;
            }

            app.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            registered = true;
            Debug.WriteLine("[StoreCrashReporting.Register] Store app crash hooks registered.");
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"[StoreCrashReporting.DispatcherUnhandledException] {e.Exception}");
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"[StoreCrashReporting.UnhandledException] {e.ExceptionObject}");
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debug.WriteLine($"[StoreCrashReporting.UnobservedTaskException] {e.Exception}");
        }
    }
}
