using System.Collections.Generic;
using UnityEditor;

namespace RemoteHierarchy.RemoteHierarchy.Editor
{
    /// <summary>
    /// A static class that manages language-specific strings for the application.
    /// </summary>
    public static class StringTable
    {
        /// <summary>
        /// Enum representing the supported languages.
        /// </summary>
        public enum LanguageType
        {
            English,
            Chinese
        }

        // Dictionaries to hold the language-specific strings.
        private static Dictionary<string, string> EnglishStrings = new Dictionary<string, string>
        {
            {"ClientAddress", "Client Address"},
            {"Connected", "Connected"},
            {"NotConnected", "Not Connected"},
            {"Disconnect", "Disconnect"},
            {"Refresh", "Refresh"},
            {"Connect", "Connect"},
            {"RemoteHierarchyViewWindowHost", "RemoteHierarchyViewWindow_Host"},
            {"Language", "Language"},
            {"English", "English"},
            {"Chinese", "Chinese"}
        };

        private static Dictionary<string, string> ChineseStrings = new Dictionary<string, string>
        {
            {"ClientAddress", "客户端地址"},
            {"Connected", "已经连接"},
            {"NotConnected", "未连接"},
            {"Disconnect", "断开"},
            {"Refresh", "刷新"},
            {"Connect", "连接"},
            {"RemoteHierarchyViewWindowHost", "RemoteHierarchyViewWindow_Host"},
            {"Language", "语言"},
            {"English", "英文"},
            {"Chinese", "中文"}
        };

        // The current language setting.
        private static LanguageType _currentLanguageType = LanguageType.English;

        /// <summary>
        /// Sets the current language.
        /// </summary>
        /// <param name="languageType">The language to set.</param>
        public static void SetLanguage(LanguageType languageType)
        {
            _currentLanguageType = languageType;
            // Store the current language in EditorPrefs
            EditorPrefs.SetInt("RemoteHierarchy_CurrentLanguage", (int)_currentLanguageType);
        }

        /// <summary>
        /// Gets the current language.
        /// </summary>
        /// <returns>The current language.</returns>
        public static LanguageType GetCurrentLanguage()
        {
            // Retrieve the current language from EditorPrefs
            if (EditorPrefs.HasKey("CurrentLanguage"))
            {
                _currentLanguageType = (LanguageType)EditorPrefs.GetInt("RemoteHierarchy_CurrentLanguage");
            }
            return _currentLanguageType;
        }

        // Properties to access the language-specific strings.
        public static string ClientAddress => GetString("ClientAddress");
        public static string Connected => GetString("Connected");
        public static string NotConnected => GetString("NotConnected");
        public static string Disconnect => GetString("Disconnect");
        public static string Refresh => GetString("Refresh");
        public static string Connect => GetString("Connect");
        public static string RemoteHierarchyViewWindowHost => GetString("RemoteHierarchyViewWindow_Host");
        public static string Language => GetString("Language");
        public static string English => GetString("English");
        public static string Chinese => GetString("Chinese");

        /// <summary>
        /// Retrieves a string for the current language.
        /// </summary>
        /// <param name="key">The key of the string to retrieve.</param>
        /// <returns>The string for the current language, or the key if the string is not found.</returns>
        private static string GetString(string key)
        {
            switch (_currentLanguageType)
            {
                case LanguageType.English:
                    return EnglishStrings.ContainsKey(key) ? EnglishStrings[key] : key;
                case LanguageType.Chinese:
                    return ChineseStrings.ContainsKey(key) ? ChineseStrings[key] : key;
                default:
                    return key;
            }
        }
    }
}