using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace AudioSelector.Setting
{
    public enum SystemTheme : int
    {
        Dark,
        Light,
        Invalid
    }

    internal class SystemRegistry
    {
        public SystemRegistry()
        {

        }

        /// <summary>
        /// Get current system theme mode from registry
        /// </summary>
        /// <returns>Dark, Light or Invalid</returns>
        public static SystemTheme GetCurrentTheme()
        {
            try
            {
                using RegistryKey key
                    = Registry
                        .CurrentUser
                        .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
                int s = (int)key?.GetValue("SystemUsesLightTheme");
                //if (!string.IsNullOrWhiteSpace(s))
                {
                    return s == 1 ? SystemTheme.Light : SystemTheme.Dark;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ex.Message}");
            }

            return SystemTheme.Invalid;
        }

    }
}
