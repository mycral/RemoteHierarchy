using System.Collections.Generic;
using UnityEditor;

namespace RemoteHierarchy.RemoteHierarchy.Editor
{
    /// <summary>
    /// 为不同语言提供一组字符串资源。
    /// </summary>
    public static class StringTable
    {
        /// <summary>
        /// 表示可用的语言类型。
        /// </summary>
        public enum LanguageType
        {
            English, // 英文
            Chinese // 中文
        }

        private static Dictionary<string, string> EnglishStrings = new Dictionary<string, string>
        {
            {"ClientAddress", "Client Address"}, // 客户端地址
            {"Connected", "Connected"}, // 已经连接
            {"NotConnected", "Not Connected"}, // 未连接
            {"Disconnect", "Disconnect"}, // 断开
            {"Refresh", "Refresh"}, // 刷新
            {"Connect", "Connect"}, // 连接
            {"RemoteHierarchyViewWindowHost", "RemoteHierarchyViewWindow_Host"}, // 远程层级视图窗口_主机
            {"Language", "Language"}, // 语言
            {"English", "English"}, // 英文
            {"Chinese", "Chinese"} // 中文
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

        /// <summary>
        /// 设置当前的语言类型。
        /// </summary>
        /// <param name="languageType">要设置的语言类型。</param>
        public static void SetLanguage(LanguageType languageType)
        {
            EditorPrefs.SetInt("RemoteHierarchy_CurrentLanguage", (int)languageType);
        }

        /// <summary>
        /// 获取当前的语言类型。
        /// </summary>
        /// <returns>当前的语言类型。</returns>
        public static LanguageType GetCurrentLanguage()
        {
            return (LanguageType)EditorPrefs.GetInt("RemoteHierarchy_CurrentLanguage",(int)LanguageType.English);
        }

        /// <summary>
        /// 获取 "ClientAddress" 键的字符串资源。
        /// </summary>
        public static string ClientAddress => GetString("ClientAddress"); // 客户端地址

        /// <summary>
        /// 获取 "Connected" 键的字符串资源。
        /// </summary>
        public static string Connected => GetString("Connected"); // 已经连接

        /// <summary>
        /// 获取 "NotConnected" 键的字符串资源。
        /// </summary>
        public static string NotConnected => GetString("NotConnected"); // 未连接

        /// <summary>
        /// 获取 "Disconnect" 键的字符串资源。
        /// </summary>
        public static string Disconnect => GetString("Disconnect"); // 断开

        /// <summary>
        /// 获取 "Refresh" 键的字符串资源。
        /// </summary>
        public static string Refresh => GetString("Refresh"); // 刷新

        /// <summary>
        /// 获取 "Connect" 键的字符串资源。
        /// </summary>
        public static string Connect => GetString("Connect"); // 连接

        /// <summary>
        /// 获取 "RemoteHierarchyViewWindowHost" 键的字符串资源。
        /// </summary>
        public static string RemoteHierarchyViewWindowHost => GetString("RemoteHierarchyViewWindow_Host"); // 远程层级视图窗口_主机

        /// <summary>
        /// 获取 "Language" 键的字符串资源。
        /// </summary>
        public static string Language => GetString("Language"); // 语言

        /// <summary>
        /// 获取 "English" 键的字符串资源。
        /// </summary>
        public static string English => GetString("English"); // 英文

        /// <summary>
        /// 获取 "Chinese" 键的字符串资源。
        /// </summary>
        public static string Chinese => GetString("Chinese"); // 中文
        
        private static string GetString(string key)
        {
            switch (GetCurrentLanguage())
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
