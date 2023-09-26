#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DebugInspector.Utils
{
    public class AllDebugsViewer : EditorWindow
    {
        private List<DataUtil.DebugInfo> debugStatements;
        public List<DataUtil.DebugInfo> DebugStatements
        {
            set => debugStatements = value;
        }
        
        private Dictionary<DataUtil.DebugInfo, bool> hideDebugsMap = new Dictionary<DataUtil.DebugInfo, bool>();

        private Vector2 scrollPosition;
        
        private GUIStyle wrapStyle;
        private GUIStyle darkBoxStyle;
        private GUIStyle toggleStyle;
        private GUIStyle separatorStyle;
        private Color separatorColor;
        
        private string scriptPath;
        
        private bool hideDebugs = false;
        private void OnEnable()
        {
            separatorStyle = new GUIStyle
            {
                normal = { background = EditorGUIUtility.whiteTexture },
                margin = new RectOffset(0, 0, 2, 2),
                fixedHeight = 2
            };
            separatorColor = new Color(0.7f, 0.7f, 0.7f);
        }

        public static void OpenWindow(string scriptPath, List<DataUtil.DebugInfo> debugStatements)
        {
            AllDebugsViewer window = GetWindow<AllDebugsViewer>("Debug Viewer");
            window.scriptPath = scriptPath;
            window.debugStatements = debugStatements;
            window.Show();
            DebugInspector.AllDebugsViewer = window;
        }
        
        private void OnGUI()
        {
            if (wrapStyle == null || darkBoxStyle == null || toggleStyle == null)
            {
                InitializeStyles();
            }
    
            GUILayout.Space(10);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); 

            int debugIndex = 1; 
            foreach (var debugInfo in debugStatements)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
    
                EditorGUILayout.LabelField($"[Index {debugIndex}]  [Line {debugInfo.LineNumber}]", GUILayout.ExpandWidth(false));

                GUILayout.FlexibleSpace();

                bool shouldHide = EditorPrefs.GetBool($"DebugHide_{debugInfo.LineNumber}", false);
                GUILayout.Label("Hide", GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("Hide")).x + 3));
                shouldHide = GUILayout.Toggle(shouldHide, "");
    
                EditorGUILayout.EndHorizontal();

                EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, separatorStyle.fixedHeight), separatorColor);
                EditorPrefs.SetBool($"DebugHide_{debugInfo.LineNumber}", shouldHide);

                if (!shouldHide)
                {
                    float widthForDebugStatement = this.position.width - 20;
                    float height = GUIUtil.GetOrCalculateTextFieldHeight(debugInfo, wrapStyle, widthForDebugStatement);
                    EditorGUILayout.LabelField(debugInfo.Statement, wrapStyle, GUILayout.Height(height), GUILayout.Width(widthForDebugStatement));
                }
                
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
                debugIndex++; 
            }
            
            GUILayout.Space(5);

            if (GUILayout.Button("Delete All Debugs"))
            {
                DeleteAllDebugs();
            }
    
            EditorGUILayout.EndScrollView(); 
        }

        private void DeleteAllDebugs()
        {
            string content = File.ReadAllText(scriptPath);
            StringBuilder newContent = new StringBuilder(content);

            foreach (var debugStatement in debugStatements)
            {
                newContent.Replace(debugStatement.Statement, "");
            }

            File.WriteAllText(scriptPath, newContent.ToString());
            debugStatements.Clear();
            DebugInspector.Instance.RefreshScriptList();
            DebugInspector.Instance.Repaint();
            Close();
        }
     
        private void InitializeStyles()
        {
            wrapStyle = new GUIStyle(EditorStyles.label) { wordWrap = true };
            darkBoxStyle = GUIUtil.DarkBoxStyle();
            toggleStyle = new GUIStyle(EditorStyles.toggle);
        }
        
        public void CloseWindow(bool click)
        {
            foreach (var debugInfo in debugStatements)
            {
                EditorPrefs.DeleteKey($"DebugHide_{debugInfo.LineNumber}");
            }
            Close();
        }
        
        public void RefreshDebugStatements(List<DataUtil.DebugInfo> debugStatements)
        {
            DebugStatements = debugStatements;
            Repaint();  
        }
    }
}
#endif