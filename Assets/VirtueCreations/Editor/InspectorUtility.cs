using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIRTUE {
    public class InspectorUtility {
        static EditorWindow _mouseOverWindow;

        [Shortcut ("Inspector Utility/Select Inspector")]
        static void SelectLockableInspector () {
            if (EditorWindow.mouseOverWindow.GetType ().Name == "InspectorWindow") {
                _mouseOverWindow = EditorWindow.mouseOverWindow;
                Type type = Assembly.GetAssembly (typeof (Editor)).GetType ("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll (type);
                int indexOf = findObjectsOfTypeAll.ToList ().IndexOf (_mouseOverWindow);
                EditorPrefs.SetInt ("LockableInspectorIndex", indexOf);
            }
        }

        [Shortcut ("Inspector Utility/Toggle Lock Inspector")]
        static void ToggleInspectorLock () {
            if (_mouseOverWindow == null) {
                if (!EditorPrefs.HasKey ("LockableInspectorIndex")) EditorPrefs.SetInt ("LockableInspectorIndex", 0);
                int i = EditorPrefs.GetInt ("LockableInspectorIndex");
                Type type = Assembly.GetAssembly (typeof (Editor)).GetType ("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll (type);
                _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
            }
            if (_mouseOverWindow != null && _mouseOverWindow.GetType ().Name == "InspectorWindow") {
                Type type = Assembly.GetAssembly (typeof (Editor)).GetType ("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty ("isLocked");
                bool value = (bool)propertyInfo.GetValue (_mouseOverWindow, null);
                propertyInfo.SetValue (_mouseOverWindow, !value, null);
                _mouseOverWindow.Repaint ();
            }
        }

        [Shortcut ("Inspector Utility/Clear Console")]
        static void ClearConsole () {
            var assembly = Assembly.GetAssembly (typeof (SceneView));
            var type = assembly.GetType ("UnityEditor.LogEntries");
            var method = type.GetMethod ("Clear");
            method.Invoke (new object (), null);
        }

        [Shortcut ("Inspector Utility/Toggle Inspector Mode")] //Change the shortcut here
        public static void ToggleInspectorDebug () {
            if (_mouseOverWindow == null) {
                if (!EditorPrefs.HasKey ("LockableInspectorIndex")) EditorPrefs.SetInt ("LockableInspectorIndex", 0);
                int i = EditorPrefs.GetInt ("LockableInspectorIndex");
                Type type = Assembly.GetAssembly (typeof (Editor)).GetType ("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll (type);
                _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
            }

            if (_mouseOverWindow != null && _mouseOverWindow.GetType ().Name == "InspectorWindow") {
                Type type = Assembly.GetAssembly (typeof (Editor)).GetType ("UnityEditor.InspectorWindow"); //Get the type of the inspector window to find out the variable/method from
                FieldInfo field = type.GetField ("m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance); //get the field we want to read, for the type (not our instance)
                InspectorMode mode = (InspectorMode)field.GetValue (_mouseOverWindow); //read the value for our target inspector
                mode = (mode == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal); //toggle the value
                MethodInfo method = type.GetMethod ("SetMode", BindingFlags.NonPublic | BindingFlags.Instance); //Find the method to change the mode for the type
                method.Invoke (_mouseOverWindow, new object[] { mode }); //Call the function on our targetInspector, with the new mode as an object[]
                _mouseOverWindow.Repaint (); //refresh inspector
            }
        }
    }
}