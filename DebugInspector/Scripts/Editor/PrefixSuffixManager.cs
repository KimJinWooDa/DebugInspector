#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;


using System.Collections.Generic;


namespace DebugInspector.Utils
{
    public class PrefixSuffixManager : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string newPrefix = "";
        private string newSuffix = "";

        public static void OpenWindow()
        {
            GetWindow(typeof(PrefixSuffixManager), false, "Prefix & Suffix Manager");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            bool shouldRefreshScriptList = false;

            HandlePrefixManagement(ref shouldRefreshScriptList);

            EditorGUILayout.Space();

            HandleSuffixManagement(ref shouldRefreshScriptList);

            EditorGUILayout.EndScrollView();

            if (shouldRefreshScriptList)
            {
                DebugInspector.Instance.RefreshScriptList();
            }
        }

        private void HandlePrefixManagement(ref bool shouldRefresh)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Custom Debug Prefix Management", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Add Custom Debug Prefix :", GUILayout.Width(160));
            newPrefix = EditorGUILayout.TextField(newPrefix);

            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                if (!string.IsNullOrWhiteSpace(newPrefix) && !DataUtil.CustomDebugPrefixes.Contains(newPrefix))
                {
                    DataUtil.CustomDebugPrefixes.Add(newPrefix);
                    DataUtil.SaveCustomDebugPrefixes();
                    DebugInspector.Instance.RefreshScriptList();
                    shouldRefresh = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Debug Prefixes List", EditorStyles.boldLabel);

            List<string> prefixesToRemove = new List<string>();
            foreach (var prefix in DataUtil.CustomDebugPrefixes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(prefix);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    DebugInspector.Instance.RefreshScriptList();
                    prefixesToRemove.Add(prefix);
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var prefix in prefixesToRemove)
            {
                DataUtil.CustomDebugPrefixes.Remove(prefix);
            }

            if (prefixesToRemove.Count > 0)
            {
                DataUtil.SaveCustomDebugPrefixes();
                shouldRefresh = true;
            }

            EditorGUILayout.EndVertical();
        }

        private void HandleSuffixManagement(ref bool shouldRefresh)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Custom Debug Suffixes Management", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Add Custom Debug Suffix :", GUILayout.Width(160));
            newSuffix = EditorGUILayout.TextField(newSuffix);

            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                if (!string.IsNullOrWhiteSpace(newSuffix) && !DataUtil.CustomDebugSuffixes.Contains(newSuffix))
                {
                    DataUtil.CustomDebugSuffixes.Add(newSuffix);
                    DataUtil.SaveCustomDebugSuffixes();
                    DebugInspector.Instance.RefreshScriptList();
                    shouldRefresh = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Debug Suffixes List", EditorStyles.boldLabel);

            List<string> suffixesToRemove = new List<string>();
            foreach (var suffix in DataUtil.CustomDebugSuffixes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(suffix);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    DebugInspector.Instance.RefreshScriptList();
                    suffixesToRemove.Add(suffix);
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var suffix in suffixesToRemove)
            {
                DataUtil.CustomDebugSuffixes.Remove(suffix);
            }

            if (suffixesToRemove.Count > 0)
            {
                DataUtil.SaveCustomDebugSuffixes();
                shouldRefresh = true;
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif