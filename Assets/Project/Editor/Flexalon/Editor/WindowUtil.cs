using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Flexalon.Editor
{
    internal static class WindowUtil
    {
        private static readonly string _projectMeta = "5325f2ad02f14e242b86eb4bb0fcb5ee";

        private static string _version;

        private static Texture2D _flexalonIcon;
        private static Texture2D _proximaIcon;

        public static void CenterOnEditor(EditorWindow window)
        {
            var editorGUIUtilityType = typeof(EditorGUIUtility);
            var getMainWindowPositionMethod = editorGUIUtilityType.GetMethod("GetMainWindowPosition",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (getMainWindowPositionMethod == null) return; // Doesn't exist in 2019.4

            var main = (Rect)getMainWindowPositionMethod.Invoke(null, null);
            var pos = window.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;
        }

        public static string GetVersion()
        {
            if (_version == null)
            {
                var version = AssetDatabase.GUIDToAssetPath(_projectMeta);
                var lines = File.ReadAllText(version);
                var rx = new Regex("\"version\": \"(.*?)\"");
                _version = rx.Match(lines).Groups[1].Value;
            }

            return _version;
        }

        public static void DrawFlexalonIcon(float width)
        {
            if (!_flexalonIcon)
            {
                var flexalonIconPath = AssetDatabase.GUIDToAssetPath("d0d1cda04ee3f144abf998efbfdfb8dc");
                _flexalonIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(flexalonIconPath);
            }

            GUILayout.Label(_flexalonIcon, GUILayout.Width(width), GUILayout.Height(width * 0.361f));
        }

        public static bool DrawProximaButton(float width, GUIStyle style)
        {
            if (!_proximaIcon)
            {
                var proximaIconPath = AssetDatabase.GUIDToAssetPath("34efc6ae99ff42f438800428a52c50b5");
                _proximaIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(proximaIconPath);
            }

            return GUILayout.Button(_proximaIcon, style, GUILayout.Width(width), GUILayout.Height(width * 0.337f));
        }
    }
}