using System.Collections.Generic;

namespace RemoteHierarchy.RemoteHierarchy.Editor
{
    public static class StringTable
    {
        public enum LanguageType
        {
            English,
            Chinese
        }

        private static Dictionary<string, string> EnglishStrings = new Dictionary<string, string>
        {
            {"ClientAddress", "Client Address"},
            {"Connected", "Connected"},
            {"NotConnected", "Not Connected"},
            {"Disconnect", "Disconnect"},
            {"Refresh", "Refresh"},
            {"Connect", "Connect"},
            {"RemoteHierarchyViewWindowHost", "Remote Hierarchy View Window Host"},
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

        private static LanguageType _currentLanguageType = LanguageType.English;

        public static void SetLanguage(LanguageType languageType)
        {
            _currentLanguageType = languageType;
        }
        public static LanguageType GetCurrentLanguage()
        {
            return _currentLanguageType;
        }
        public static string ClientAddress => GetString("ClientAddress");
        public static string Connected => GetString("Connected");
        public static string NotConnected => GetString("NotConnected");
        public static string Disconnect => GetString("Disconnect");
        public static string Refresh => GetString("Refresh");
        public static string Connect => GetString("Connect");
        public static string RemoteHierarchyViewWindowHost => GetString("RemoteHierarchyViewWindowHost");
        public static string Language => GetString("Language");
        public static string English => GetString("English");
        public static string Chinese => GetString("Chinese");
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