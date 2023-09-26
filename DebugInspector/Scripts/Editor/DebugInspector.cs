#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DebugInspector.Utils
{
    public class DebugInspector : EditorWindow
    {
        public static DebugInspector Instance;
        public static AllDebugsViewer AllDebugsViewer;
        
        private Vector2 scrollPosition;

        private readonly Dictionary<string, List<DataUtil.DebugInfo>> debugStatements = new Dictionary<string, List<DataUtil.DebugInfo>>();
        private readonly Dictionary<string, int> debugStatementIndices = new Dictionary<string, int>();
        private readonly Dictionary<string, string> editedDebugStatements = new Dictionary<string, string>();

        private Texture2D nextDebugIcon;
        private Texture2D previousDebugIcon;
        
        private string[] folders;
        private string[] scripts;
         
        private string selectedFolder;
        private string searchQuery = "";

        private int selectedFolderIndex = 0;

        private bool ascendingOrder = true;
        private bool changesMade = false;
        
        [MenuItem("Window/Debug Inspector")]
        public static void ShowWindow()
        {
            GetWindow<DebugInspector>("Debug Inspector");
        }

        private void OnEnable()
        {
            Instance = this;
            folders = GetAllFolders();
            
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)));

            string GetIconPath(string iconName)
            {
                return Path.Combine(rootDirectory, "Icons", iconName + ".png").Replace("\\", "/");
            }

            nextDebugIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath("Next"));
            previousDebugIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(GetIconPath("Previous"));

            selectedFolderIndex = EditorPrefs.GetInt("DebugInspector_SelectedFolderIndex", 0);

            if (folders.Length > 0 && selectedFolderIndex < folders.Length)
            {
                selectedFolder = folders[selectedFolderIndex];
            }
            
            DataUtil.LoadCustomDebugPrefixes();
            DataUtil.LoadCustomDebugSuffixes();
            RefreshScriptList();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Folder:", GUILayout.Width(85));

            float maxFolderNameWidth = folders.Max(f => GUI.skin.label.CalcSize(new GUIContent(f)).x);
            int newSelectedFolderIndex =
                EditorGUILayout.Popup(selectedFolderIndex, folders, GUILayout.Width(maxFolderNameWidth + 30));

            if (newSelectedFolderIndex != selectedFolderIndex)
            {
                selectedFolderIndex = newSelectedFolderIndex;
                selectedFolder = folders[selectedFolderIndex];
                RefreshScriptList();
                Repaint();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
            searchQuery = EditorGUILayout.TextField(searchQuery, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var filteredScripts = debugStatements
                .Where(script => Path.GetFileName(script.Key).IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToDictionary(script => script.Key, script => script.Value);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label($"[Total Filtered Scripts: {filteredScripts.Count}]", GUILayout.Width(200), GUILayout.ExpandWidth(true));
            
            GUILayout.FlexibleSpace();
            
            string manageButtonTxt = "Manage Prefixes & Suffixes";
            float manageButtonWidth = GUI.skin.button.CalcSize(new GUIContent(manageButtonTxt)).x;
            if (GUILayout.Button(manageButtonTxt, GUILayout.Width(manageButtonWidth + 10), GUILayout.ExpandWidth(true)))
            {
                PrefixSuffixManager.OpenWindow();
            }
            
            GUILayout.FlexibleSpace();
            
            string buttonText = ascendingOrder ? "Sort Descending" : "Sort Ascending";
            float buttonWidth = GUI.skin.button.CalcSize(new GUIContent(buttonText)).x;
            if (GUILayout.Button(buttonText, GUILayout.Width(buttonWidth + 10), GUILayout.ExpandWidth(true))) 
            {
                ascendingOrder = !ascendingOrder;
            }

            EditorGUILayout.EndHorizontal();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var sortedAndFilteredScripts = ascendingOrder 
                ? filteredScripts.OrderBy(script => Path.GetFileName(script.Key), StringComparer.OrdinalIgnoreCase).ToDictionary(script => script.Key, script => script.Value)
                : filteredScripts.OrderByDescending(script => Path.GetFileName(script.Key), StringComparer.OrdinalIgnoreCase).ToDictionary(script => script.Key, script => script.Value);
           
            
            foreach (var script in sortedAndFilteredScripts)
            {
                if (!debugStatements.ContainsKey(script.Key) || debugStatements[script.Key].Count == 0)
                    continue;

                int debugIndex = debugStatementIndices.ContainsKey(script.Key) ? debugStatementIndices[script.Key] : 0;
                int currentDebugIndex = debugIndex + 1; 

                if (!debugStatements.ContainsKey(script.Key) || debugStatements[script.Key].Count == 0)
                    continue;
                
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(GUI.skin.box);

                string scriptName = Path.GetFileName(script.Key);
                GUILayout.Label(scriptName, EditorStyles.boldLabel);
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open Script", GUILayout.Width(120), GUILayout.Height(20)))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(script.Key, 0);
                }

                EditorGUI.BeginDisabledGroup(debugIndex <= 0 || !previousDebugIcon);
                {
                    GUIContent btnContent = new GUIContent(previousDebugIcon ?? Texture2D.blackTexture,
                        "View Previous Debug");
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        debugStatementIndices[script.Key] = debugIndex - 1;
                        currentDebugIndex--; 
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(debugIndex >= script.Value.Count - 1 || !nextDebugIcon);
                {
                    GUIContent btnContent = new GUIContent(nextDebugIcon ?? Texture2D.blackTexture, "View Next Debug");
                    if (GUILayout.Button(btnContent, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        debugStatementIndices[script.Key] = debugIndex + 1;
                        currentDebugIndex++; 
                    }
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("View All Debugs", GUILayout.Width(120), GUILayout.Height(20)))
                {
                    List<DataUtil.DebugInfo> displayStatements = debugStatements[script.Key].ToList();
                    AllDebugsViewer.OpenWindow(script.Key, displayStatements);
                }

                EditorGUILayout.EndHorizontal();


                GUIStyle centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };

                EditorGUILayout.BeginVertical(GUIUtil.GuiTextFieldStyle(), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField(
                    new GUIContent($"[{currentDebugIndex}] Debug Statement", "This is the debug statement from the script."),
                    centeredBoldLabel);

                DataUtil.DebugInfo currentDebugInfo = script.Value[debugIndex];
                string debugStatement = currentDebugInfo.Statement;

                GUIStyle wrapStyle = new GUIStyle(EditorStyles.textField) { wordWrap = true };

                float height = GUIUtil.GetOrCalculateTextFieldHeight(currentDebugInfo, wrapStyle, this.position.width);
                string editedDebug = EditorGUILayout.TextField(debugStatement, wrapStyle, GUILayout.ExpandWidth(true),
                    GUILayout.Height(height));

                if (editedDebug != debugStatement)
                {
                    currentDebugInfo.Statement = editedDebug;
                    editedDebugStatements[debugStatement] = editedDebug;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                if (editedDebugStatements.ContainsKey(debugStatement) && GUILayout.Button("Save Changes", GUILayout.Width(100)))
                {
                    SaveEditedDebugStatement(script.Key, currentDebugInfo, editedDebugStatements[debugStatement]);

                    var updatedDebugInfo = script.Value[debugIndex];
                    updatedDebugInfo.Statement = editedDebugStatements[debugStatement];
                    script.Value[debugIndex] = updatedDebugInfo;
                    
                    editedDebugStatements.Remove(debugStatement);

                    AllDebugsViewer?.RefreshDebugStatements(debugStatements[script.Key].ToList());

                    changesMade = true;
                }
                
                GUILayout.FlexibleSpace();

                GUIContent removeBtnContent = new GUIContent("Remove", "Click to remove the Debug");

                if (GUILayout.Button(removeBtnContent, GUILayout.Width(100), GUILayout.Height(25)))
                {
                    RemoveDebugInScript(script.Key, currentDebugInfo);
                    script.Value.RemoveAt(debugIndex);
                    if (script.Value.Count == 0)
                    {
                        debugStatementIndices.Remove(script.Key);
                    }
                    else
                    {
                        debugStatementIndices[script.Key] = Mathf.Clamp(debugStatementIndices[script.Key], 0,
                            script.Value.Count - 1);
                    }

                    AllDebugsViewer?.RefreshDebugStatements(debugStatements[script.Key].ToList());
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            
            if (changesMade)
            {
                GUIStyle saveButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    normal =
                    {
                        textColor = Color.white,
                        background = GUIUtil.MakeTex(2, 2, Color.red)
                    },
                    hover =
                    {
                        textColor = Color.white,
                        background = GUIUtil.MakeTex(2, 2, new Color(0.8f, 0.1f, 0.1f))
                    },
                    active =
                    {
                        textColor = Color.white,
                        background = GUIUtil.MakeTex(2, 2, new Color(0.7f, 0.1f, 0.1f))
                    },
                    fontSize = 12
                };

                if (GUILayout.Button("Refresh", saveButtonStyle))
                {
                    AllDebugsViewer?.CloseWindow(false);
                    AssetDatabase.Refresh();
                    changesMade = false;
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void SaveEditedDebugStatement(string scriptPath, DataUtil.DebugInfo originalDebugInfo, string editedDebug)
        {
            string content = File.ReadAllText(scriptPath);
            content = content.Replace(originalDebugInfo.Statement, editedDebug);
            File.WriteAllText(scriptPath, content);
        }
        
        private void RemoveDebugInScript(string scriptPath, DataUtil.DebugInfo debugInfoToRemove)
        {
            string content = File.ReadAllText(scriptPath);
            content = content.Replace(debugInfoToRemove.Statement, "");
            File.WriteAllText(scriptPath, content);
        }
        
        private string[] GetAllFolders()
        {
            string[] allFolders = Directory.GetDirectories(Application.dataPath, "*", SearchOption.TopDirectoryOnly);
            return allFolders.Select(folder => folder.Replace(Application.dataPath + Path.DirectorySeparatorChar, "")).ToArray();
        }
        
        public void RefreshScriptList()
        {
            string suffixPattern = DataUtil.CustomDebugSuffixes.Count > 0 ? string.Join("|", DataUtil.CustomDebugSuffixes) : "Log|LogWarning|LogError";
            string pattern = $@"(//\s*)*({string.Join("|", DataUtil.CustomDebugPrefixes)})\.({suffixPattern})\(([^;]+)\);";

            scripts = Directory.GetFiles(Path.Combine(Application.dataPath, selectedFolder), "*.cs", SearchOption.AllDirectories);

            var regexOptions = RegexOptions.IgnoreCase;
            scripts = Directory.GetFiles(Path.Combine(Application.dataPath, selectedFolder), "*.cs", SearchOption.AllDirectories);

            debugStatements.Clear();
            debugStatementIndices.Clear();
            
            foreach (var script in scripts)
            {
                var content = File.ReadAllLines(script);
                List<DataUtil.DebugInfo> debugInfos = new List<DataUtil.DebugInfo>();
                for (int i = 0; i < content.Length; i++)
                {
                    var match = Regex.Match(content[i], pattern, regexOptions);
                    if (match.Success)
                    {
                        debugInfos.Add(new DataUtil.DebugInfo { LineNumber = i + 1, Statement = match.Value });
                    }
                }

                if (debugInfos.Any())
                {
                    debugStatements[script] = debugInfos;
                    debugStatementIndices[script] = 0;
                }
            }
        }
    }
}
#endif