#if UNITY_EDITOR
using UnityEngine;

namespace DebugInspector.Utils
{
    public static class GUIUtil
    {
        private static GUIStyle customBoxStyle;
        private static GUIStyle darkBoxStyle;
        private static Texture2D darkBoxTexture;
        private static Texture2D customBoxTexture;

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static GUIStyle DarkBoxStyle()
        {
            if (darkBoxStyle == null)
            {
                darkBoxStyle = new GUIStyle(GUI.skin.box);
                if (darkBoxTexture == null)
                {
                    darkBoxTexture = MakeTex(256, 256, new Color(0.05f, 0.05f, 0.05f));
                }
                darkBoxStyle.normal.background = darkBoxTexture;
            }
            return darkBoxStyle;
        }
        
        public static GUIStyle GuiTextFieldStyle()
        {
            if (customBoxStyle == null)
            {
                customBoxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };

                if (customBoxTexture == null)
                {
                    customBoxTexture = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.9f));
                }
                customBoxStyle.normal.background = customBoxTexture;
            }
            return customBoxStyle;
        }
        
        public static float GetOrCalculateTextFieldHeight(DataUtil.DebugInfo debugInfo, GUIStyle style, float width)
        {
            if (!DataUtil.DebugStatementHeights.ContainsKey(debugInfo))
            {
                GUIContent content = new GUIContent(debugInfo.Statement);
                int lines = Mathf.CeilToInt(style.CalcHeight(content, width) / style.lineHeight);
                DataUtil.DebugStatementHeights[debugInfo] = lines * style.lineHeight + 10;
            }
            return DataUtil.DebugStatementHeights[debugInfo];
        }

    }
}
#endif