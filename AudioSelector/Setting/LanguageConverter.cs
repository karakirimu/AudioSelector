using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace AudioSelector.Setting
{

    internal class LanguageConverter
    {
        private static readonly IReadOnlyDictionary<int, string> SupportedLCID = new Dictionary<int, string> {
            { 1033, "en-US" }, // English - United States
            { 1041, "ja-JP" } // Japanese - Japan
        };

        /// <summary>
        /// Get supported language code
        /// </summary>
        /// <param name="language"></param>
        /// <returns>LanguageCode if supported, it returns supported language, otherwise en-US.</returns>
        public static string GetSupportedLanguageCode(string language)
        {
            switch (language)
            {
                case "English":
                    return "en-US";
                case "日本語":
                    return "ja-JP";
                case "System":
                default:
                    int lcid = NativeMethods.GetUserDefaultLCID();
                    if (SupportedLCID.ContainsKey(lcid))
                    {
                        return SupportedLCID[lcid];
                    }
                    else
                    {
                        return "en-US";
                    }
            }
        }

        /// <summary>
        /// Get ComboBox index from supported language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static int GetIndexFromSupportedLanguage(string language)
        {
            return language switch
            {
                "System" => 0,
                "English" => 1,
                "日本語" => 2,
                _ => 0,
            };
        }

        /// <summary>
        /// Get supported language from language code
        /// </summary>
        /// <param name="languageSetting">LanguageCode code</param>
        /// <returns>Supported language or System</returns>
        public static string GetSupportedLanguage(string languageSetting)
        {
            var resources = Application.Current.Resources;
            if (resources.Contains("LanguageSystem"))
            {
                if (languageSetting == resources["LanguageSystem"].ToString())
                {
                    return "System";
                }
            }

            // other languages
            return languageSetting;
        }
    }
}
