using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace AudioSourceSelector
{
    public enum CurrentTheme : int
    {
        Dark,
        Light,
        Invalid
    }

    internal class SystemRegistrySetting
    {
        public SystemRegistrySetting()
        {

        }

        /// <summary>
        /// Get current system theme mode from registry
        /// </summary>
        /// <returns>Dark, Light or Invalid</returns>
        public static CurrentTheme GetCurrentTheme()
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
                    return s == 1 ? CurrentTheme.Light : CurrentTheme.Dark;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ex.Message}");
            }

            return CurrentTheme.Invalid;
        }

    }
}
