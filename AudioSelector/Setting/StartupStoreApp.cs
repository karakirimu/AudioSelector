using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;

namespace AudioSelector.Setting
{
    internal class StartupStoreApp
    {
        private const string StartupTaskId = "karakirimu.AudioSelector.{8AA9A6A7-91C7-4EF1-98C8-ADEE97E35792}";

        public static bool IsStoreApp()
        {
            try
            {
                var package = Package.Current;
                return package.Id != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        public static async Task<bool> CheckStartupEntry()
        {
            try
            {
                StartupTask startupTask = await StartupTask.GetAsync(StartupTaskId);
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        Debug.WriteLine("[StartupStoreApp.CheckStartupEntry] Startup task is disabled.");
                        return false;
                    case StartupTaskState.DisabledByUser:
                        Debug.WriteLine("[StartupStoreApp.CheckStartupEntry] Startup task is disabled by user.");
                        return false;
                    case StartupTaskState.DisabledByPolicy:
                        Debug.WriteLine("[StartupStoreApp.CheckStartupEntry] Startup task is disabled by policy.");
                        return false;
                    case StartupTaskState.Enabled:
                        Debug.WriteLine("[StartupStoreApp.CheckStartupEntry] Startup task is enabled.");
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }

            return false;
        }

        public static async Task<bool> EnableStartupTask()
        {
            StartupTask startupTask = await StartupTask.GetAsync(StartupTaskId);

            if (startupTask.State == StartupTaskState.Disabled
                || startupTask.State == StartupTaskState.DisabledByUser)
            {
                StartupTaskState newState = await startupTask.RequestEnableAsync();
                if (newState == StartupTaskState.Enabled)
                {
                    Debug.WriteLine("[StartupStoreApp.EnableStartupTask] Startup task is now enabled.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("[StartupStoreApp.EnableStartupTask] Startup task could not be enabled.");
                    return false;
                }
            }

            if (startupTask.State == StartupTaskState.DisabledByPolicy)
            {
                string exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);
                _ = MessageBox.Show(
                        Properties.Resources.StartupDisabledByPolicy,
                        exeName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                Debug.WriteLine("[StartupStoreApp.EnableStartupTask] Startup task is disabled by policy.");
                return false;
            }

            return startupTask.State == StartupTaskState.Enabled;
        }

        public static async Task<bool> DisableStartupTask()
        {
            StartupTask startupTask = await StartupTask.GetAsync(StartupTaskId);
            if (startupTask.State == StartupTaskState.Enabled)
            {
                startupTask.Disable();
                return true;
            }

            return await CheckStartupEntry();
        }
    }
}
