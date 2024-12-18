using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace AudioSelector.Setting
{
    public enum SystemTheme : int
    {
        System,
        Light,
        Dark
    }

    internal class SystemRegistry
    {
        public SystemRegistry()
        {

        }

        /// <summary>
        /// Get current system theme mode from registry
        /// </summary>
        /// <returns>Dark, Light or System</returns>
        public static SystemTheme GetCurrentTheme()
        {
            try
            {
                using RegistryKey key
                    = Registry
                        .CurrentUser
                        .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
                int s = (int)key?.GetValue("SystemUsesLightTheme");
                return s == 1 ? SystemTheme.Light : SystemTheme.Dark;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ex.Message}");
            }

            return SystemTheme.System;
        }

        public static void RegisterStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var path= Environment.ProcessPath;
            registryKey.SetValue(assembly.GetName().Name, path);
            registryKey.Close();
        }

        public static bool HasStartupEntry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return registryKey.GetValue(assembly.GetName().Name) != null;
        }

        public static void UnregisterStartup()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            registryKey.DeleteValue(assembly.GetName().Name, false);
            registryKey.Close();
        }

    }
}
