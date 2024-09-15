using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace VIRTUE {
    [EditorTool ("Selection Tool")]
    public class SelectionTool : EditorTool {
        // Serialize this value to set a default value in the Inspector.
        [SerializeField]
        Texture2D m_ToolIcon;

        GUIContent m_IconContent;

        object _sceneOverlayWindow;
        MethodInfo _showSceneViewOverlay;

        void OnEnable () {
            m_IconContent = new GUIContent {
                image = m_ToolIcon,
                text = "Selection Tool",
                tooltip = "Click on any object with collider and get it's position"
            };
            EnableSettingsWindow ();
        }

        public override GUIContent toolbarIcon => m_IconContent;

        [UnityEditor.ShortcutManagement.Shortcut ("Activate Selection Tool", KeyCode.Q)]
        static void SurfaceAlignToolShortcut () {
            ToolManager.SetActiveTool<SelectionTool> ();
        }

        static void SetPositionAndRotation (Vector3 position, Quaternion rotation) {
            foreach (var tSelected in Selection.transforms) {
                Undo.RecordObject (tSelected, "Position and Rotation Changed");
                tSelected.SetPositionAndRotation (position, rotation);
            }
        }

        static void SetPositionAndNormal (Vector3 position, Vector3 euler) {
            foreach (var tSelected in Selection.transforms) {
                Undo.RecordObject (tSelected, "Position and Rotation Changed");
                tSelected.position = position;
                tSelected.rotation = Quaternion.FromToRotation (tSelected.up, euler);
                // tSelected.rotation = Quaternion.LookRotation (euler);
            }
        }

        static void SetParent (Transform parent) {
            foreach (var tSelected in Selection.transforms) {
                if (tSelected == parent) continue;
                Undo.RecordObject (tSelected, "Parent Changed");
                tSelected.SetParent (parent);
            }
        }

        public override void OnToolGUI (EditorWindow window) {
            //If we're not in the scene view, exit.
            if (!(window is SceneView)) {
                return;
            }

            //If we're not the active tool, exit.
#if UNITY_2020_2_OR_NEWER
            if (!ToolManager.IsActiveTool (this))
#else
            if (!UnityEditor.EditorTools.EditorTools.IsActiveTool(this))
#endif
            {
                return;
            }
            if (_showSceneViewOverlay != null && _sceneOverlayWindow != null) {
                _showSceneViewOverlay.Invoke (null, new object[] { _sceneOverlayWindow });
            }
            var controlId = GUIUtility.GetControlID (GetHashCode (), FocusType.Passive);
            /*Handles.BeginGUI ();
            {
                var width = 200;
                GUILayout.BeginArea (new Rect (8, 8, width, EditorGUIUtility.singleLineHeight * 6 + 16), GUI.skin.window);
                {
                    var rect = new Rect (8, 8, width, EditorGUIUtility.singleLineHeight);
                    GUI.Label (rect, "Shift : Instant Parent to Object");
                    rect.y += EditorGUIUtility.singleLineHeight;
                    GUI.Label (rect, "Ctrl : Place at Cursor");
                    rect.y += EditorGUIUtility.singleLineHeight;
                    GUI.Label (rect, "Ctrl+Shift : Parent to Prefab");
                    rect.y += EditorGUIUtility.singleLineHeight;
                    GUI.Label (rect, "Alt : Place at Object");
                    rect.y += EditorGUIUtility.singleLineHeight;
                    GUI.Label (rect, "Alt+Shift : Place at Prefab");
                    rect.y += EditorGUIUtility.singleLineHeight;
                    GUI.Label (rect, "Alt+Ctrl : Null Parent");
                }
                GUILayout.EndArea ();
            }
            Handles.EndGUI ();*/
            var ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
            var raycast = Physics.Raycast (ray, out var hit);
            var curEvent = Event.current;
            switch (curEvent.type) {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl (controlId);
                    break;

                case EventType.MouseDown:
                    if (curEvent.button == 0 && raycast) {
                        var modifier = (int)curEvent.modifiers;
                        switch (modifier) {
                            //Shift
                            case 1:
                                SetParent (hit.transform);
                                break;

                            //Ctrl
                            case 2:
                                SetPositionAndNormal (hit.point, hit.normal);
                                break;

                            //Ctrl+Shift
                            case 3:
                                if (PrefabUtility.IsPartOfPrefabInstance (hit.collider)) {
                                    GameObject go = PrefabUtility.GetOutermostPrefabInstanceRoot (hit.collider);
                                    if (go != null) {
                                        SetParent (go.transform);
                                    }
                                } else {
                                    SetParent (hit.transform);
                                }
                                break;

                            //Alt
                            case 4:
                                SetPositionAndRotation (hit.transform.position, hit.transform.rotation);
                                break;

                            //Alt+Shift
                            case 5:
                                if (PrefabUtility.IsPartOfPrefabInstance (hit.collider)) {
                                    var go = PrefabUtility.GetOutermostPrefabInstanceRoot (hit.collider);
                                    if (go != null) {
                                        SetPositionAndRotation (go.transform.position, go.transform.rotation);
                                    }
                                } else {
                                    SetPositionAndRotation (hit.transform.position, hit.transform.rotation);
                                }
                                break;

                            //Alt+Ctrl
                            case 6:
                                SetParent (null);
                                break;
                        }
                        curEvent.Use ();
                    }
                    break;
            }
        }

        void EnableSettingsWindow () {
#if UNITY_2020_1_OR_NEWER
            Assembly unityEditor = Assembly.GetAssembly (typeof (UnityEditor.SceneView));
            Type overlayWindowType = unityEditor.GetType ("UnityEditor.OverlayWindow");
            Type sceneViewOverlayType = unityEditor.GetType ("UnityEditor.SceneViewOverlay");
            Type windowFuncType = sceneViewOverlayType.GetNestedType ("WindowFunction");
            Delegate windowFunc = Delegate.CreateDelegate (windowFuncType, this.GetType ().GetMethod (nameof (DoOverlayUI), BindingFlags.Static | BindingFlags.NonPublic));
            Type windowDisplayOptionType = sceneViewOverlayType.GetNestedType ("WindowDisplayOption");
            _sceneOverlayWindow = Activator.CreateInstance (overlayWindowType,
                                                            EditorGUIUtility.TrTextContent ("Selection Tool Info"), // Title
                                                            windowFunc, // Draw function of the window
                                                            int.MaxValue, // Priority of the window
                                                            null, // Unity Obect that will be passed to the drawing function
                                                            Enum.Parse (windowDisplayOptionType, "OneWindowPerTarget") //SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget
                                                           );
            _showSceneViewOverlay = sceneViewOverlayType.GetMethod ("ShowWindow", BindingFlags.Static | BindingFlags.Public);
#endif
        }

        static void DoOverlayUI (UnityEngine.Object settingsObject, SceneView sceneView) {
            GUILayout.Space (10);
            EditorGUILayout.LabelField ("Shift: Instant Parent to Object");
            EditorGUILayout.LabelField ("Ctrl: Place at Cursor");
            EditorGUILayout.LabelField ("Ctrl+Shift: Parent to Prefab");
            EditorGUILayout.LabelField ("Alt: Place at Object");
            EditorGUILayout.LabelField ("Alt+Shift: Place at Prefab");
            EditorGUILayout.LabelField ("Alt+Ctrl: Null Parent");
        }
    }
}