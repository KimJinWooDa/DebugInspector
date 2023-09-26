#if UNITY_EDITOR


using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor;

namespace DebugInspector.Utils
{
    public static class DataUtil
    {
        public static HashSet<string> CustomDebugPrefixes { get; set; } = new HashSet<string>();
        public static HashSet<string> CustomDebugSuffixes { get; set; } = new HashSet<string>();
        
        public static Dictionary<DebugInfo, float> DebugStatementHeights = new Dictionary<DebugInfo, float>();

        public static void LoadCustomDebugPrefixes()
        {
            string storedPrefixes = EditorPrefs.GetString("CustomDebugPrefixes", "");
            CustomDebugPrefixes = new HashSet<string>(
                storedPrefixes.Split(',')
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
            );
            CustomDebugPrefixes.Add("Debug");
        }

        public static void LoadCustomDebugSuffixes()
        {
            string storedSuffixes = EditorPrefs.GetString("CustomDebugSuffixes", "");
            CustomDebugSuffixes = new HashSet<string>(
                storedSuffixes.Split(',').Where(suffix => !string.IsNullOrWhiteSpace(suffix))
            );
            if (!CustomDebugSuffixes.Any())
            {
                CustomDebugSuffixes.Add("Log");
                CustomDebugSuffixes.Add("LogWarning");
                CustomDebugSuffixes.Add("LogError");
            }
        }

        public static void SaveCustomDebugPrefixes()
        {
            string prefixString = string.Join(",", CustomDebugPrefixes);
            EditorPrefs.SetString("CustomDebugPrefixes", prefixString);
        }

        public static void SaveCustomDebugSuffixes()
        {
            string suffixString = string.Join(",", CustomDebugSuffixes);
            EditorPrefs.SetString("CustomDebugSuffixes", suffixString);
        }

        public struct DebugInfo
        {
            public int LineNumber { get; set; }
            public string Statement { get; set; }
        }
    }
}
#endif