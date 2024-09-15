using System;
using System.IO;
using System.Linq;
using Tarodev;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using Object = UnityEngine.Object;

namespace VIRTUE {
    [InitializeOnLoad]
    public class ToolBarExtention {
        static int _selected;
        static string[] _allScenesPath;
        const float WIDTH = 80f;

        class Styles {
            public static readonly GUIContent SettingsLabel = EditorGUIUtility.TrTextContent ("", "Open Project Settings to Player", "d_Settings");
            public static readonly GUIContent AutoSaveOnLabel = EditorGUIUtility.TrTextContent ("ON", "Toggle Save ON", "SaveActive");
            public static readonly GUIContent AutoSaveOffLabel = EditorGUIUtility.TrTextContent ("OFF", "Toggle Save Off", "SaveActive");
            public static readonly GUIContent RefreshLabel = EditorGUIUtility.TrTextContent ("", "Refresh", "d_Refresh");
            public static readonly GUIContent ClearDataLabel = EditorGUIUtility.TrTextContent ("", "Clear All Data Including PlayerPrefs", "TreeEditor.Trash");
            public static readonly GUIContent ViewDataLabel = EditorGUIUtility.TrTextContent ("", "View All Data", "d_FolderOpened Icon");
            public static readonly GUIContent PingScenesDirLabel = EditorGUIUtility.TrTextContent ("", "Ping Scenes Dir", "UnityEditor.SceneView");
            public static readonly GUIContent InitializationSceneLabel = EditorGUIUtility.TrTextContent ("Init", "Open Initialization Scene", "Favorite");
            public static readonly GUIContent PersistentManagersSceneLabel = EditorGUIUtility.TrTextContent ("Manager", "Open PersistentManagers Scene", "Favorite");
            public static readonly GUIContent GameSceneLabel = EditorGUIUtility.TrTextContent ("Game", "Open GameScene", "Favorite");
            public static readonly GUIContent TestSceneLabel = EditorGUIUtility.TrTextContent ("Test", "Open TestScene", "Favorite");
        }

        static ToolBarExtention () {
            ToolbarExtender.LeftToolbarGUI.Add (OnToolbarGUILeft);
            ToolbarExtender.RightToolbarGUI.Add (OnToolbarGUIRight);
        }

        static void OnToolbarGUILeft () {
            if (GUILayout.Button (Styles.SettingsLabel, GUILayout.Width (32))) {
                SettingsService.OpenProjectSettings ("Project/Player");
            }
            if (TarodevAutoSave.CurrentState) {
                if (GUILayout.Button (Styles.AutoSaveOnLabel, GUILayout.Width (WIDTH))) {
                    TarodevAutoSave.ToggleAutoSave ();
                }
            } else {
                if (GUILayout.Button (Styles.AutoSaveOffLabel, GUILayout.Width (WIDTH))) {
                    TarodevAutoSave.ToggleAutoSave ();
                }
            }
            var isAutoRefreshOn = EditorPrefs.GetBool ("kAutoRefresh");
            if (!isAutoRefreshOn) {
                if (GUILayout.Button (Styles.RefreshLabel, GUILayout.Width (32))) {
                    AssetDatabase.Refresh ();
                }
            }
            DrawScenesDropDown ();
            GUILayout.Label ("Time Scale");
            Time.timeScale = EditorGUILayout.FloatField (Time.timeScale, GUILayout.Width (40f));
            Time.timeScale = GUILayout.HorizontalSlider (Time.timeScale, 0f, 10f, GUILayout.Width (120f));
            GUILayout.FlexibleSpace ();
        }

        static void OnToolbarGUIRight () {
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button (Styles.ClearDataLabel, GUILayout.Width (32))) {
                PlayerPrefs.DeleteAll ();
                var directoryInfo = new DirectoryInfo (Application.persistentDataPath);
                var files = directoryInfo.GetFiles ();
                foreach (var file in files) {
                    File.Delete (file.FullName);
                }
            }
            if (GUILayout.Button (Styles.ViewDataLabel, GUILayout.Width (32), GUILayout.Height (20))) {
                EditorUtility.OpenWithDefaultApp (Application.persistentDataPath);
            }
            if (GUILayout.Button (Styles.PingScenesDirLabel, GUILayout.Width (32))) {
                var scenesDir = AssetDatabase.LoadAssetAtPath<Object> ("Assets/VirtueCreations/Scenes");
                EditorGUIUtility.PingObject (scenesDir);
            }
            if (GUILayout.Button (Styles.InitializationSceneLabel, GUILayout.Width (WIDTH))) {
                OpenAndSaveCurrent ("Assets/VirtueCreations/Scenes/Initialization.unity");
            }
            if (GUILayout.Button (Styles.PersistentManagersSceneLabel, GUILayout.Width (WIDTH))) {
                OpenAndSaveCurrent ("Assets/VirtueCreations/Scenes/PersistentManagers.unity");
            }
            if (GUILayout.Button (Styles.GameSceneLabel, GUILayout.Width (WIDTH))) {
                OpenAndSaveCurrent ("Assets/VirtueCreations/Scenes/GameScene.unity");
            }
            if (GUILayout.Button (Styles.TestSceneLabel, GUILayout.Width (WIDTH))) {
                OpenAndSaveCurrent ("Assets/VirtueCreations/Scenes/TestScene.unity");
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace ();
        }

        static void DrawScenesDropDown () {
            _allScenesPath = AssetDatabase.FindAssets ("t:Scene").Select (AssetDatabase.GUIDToAssetPath).ToArray ();
            var activeSceneIndex = EditorPrefs.GetInt ("VIRTUE.CurrentOpenSceneIndex", _selected);
            if (_selected != activeSceneIndex) {
                _selected = activeSceneIndex;
            }
            using (var check = new EditorGUI.ChangeCheckScope ()) {
                _selected = EditorGUILayout.Popup (_selected, _allScenesPath, GUILayout.Width (300f));
                if (check.changed) {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
                        EditorPrefs.SetInt ("VIRTUE.CurrentOpenSceneIndex", _selected);
                        EditorSceneManager.OpenScene (_allScenesPath[_selected]);
                    }
                }
            }
        }

        static void OpenAndSaveCurrent (string scenePath) {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
                EditorSceneManager.OpenScene (scenePath);
                if (_allScenesPath.Contains (scenePath)) {
                    _selected = Array.IndexOf (_allScenesPath, scenePath);
                    EditorPrefs.SetInt ("VIRTUE.CurrentOpenSceneIndex", _selected);
                }
            }
        }
    }
}