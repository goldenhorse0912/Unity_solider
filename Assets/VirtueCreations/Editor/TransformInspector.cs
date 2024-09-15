using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIRTUE {
    // [CustomEditor (typeof (Transform), true)]
    // [CanEditMultipleObjects]
    class TransformInspector : Editor {
        Editor _defaultEditor;

        class Styles {
            public static readonly GUIContent AlignTransformLabel = EditorGUIUtility.TrTextContent ("Align Transform", "Align Transform", "TransformTool On");
            public static readonly GUIContent AlignPositionLabel = EditorGUIUtility.TrTextContent ("Align Position", "Align Position", "d_MoveTool On");
            public static readonly GUIContent AlignRotationLabel = EditorGUIUtility.TrTextContent ("Align Rotation", "Align Rotation", "d_RotateTool On");
            public static readonly GUIContent ResetPositionLabel = EditorGUIUtility.TrTextContent ("Reset Position", "Reset Transform", "d_MoveTool On");
            public static readonly GUIContent ResetRotationLabel = EditorGUIUtility.TrTextContent ("Reset Rotation", "Reset Rotation", "d_RotateTool On");
            public static readonly GUIContent ResetScaleLabel = EditorGUIUtility.TrTextContent ("Reset Scale", "Reset Scale", "d_ScaleTool On");
        }

        void OnEnable () {
            _defaultEditor = CreateEditor (targets.Where (x => x != null).ToArray (), Type.GetType ("UnityEditor.TransformInspector, UnityEditor"));
        }

        void OnDisable () {
            if (_defaultEditor != null) {
                DestroyImmediate (_defaultEditor);
            }
        }

        protected override void OnHeaderGUI () {
            _defaultEditor.DrawHeader ();
        }

        public override void OnInspectorGUI () {
            var width = Screen.width - 28f;
            width /= 3f;
            if (Developer.ValidateAlign ()) {
                using (new EditorGUILayout.HorizontalScope ()) {
                    if (GUILayout.Button (Styles.AlignTransformLabel, GUILayout.Width (width))) {
                        Developer.PerformTransformAlign ();
                    }
                    if (GUILayout.Button (Styles.AlignPositionLabel, GUILayout.Width (width))) {
                        Developer.PerformPositionAlign ();
                    }
                    if (GUILayout.Button (Styles.AlignRotationLabel, GUILayout.Width (width))) {
                        Developer.PerformRotationAlign ();
                    }
                }
                GUILayout.Space (2);
            }
            _defaultEditor.OnInspectorGUI ();
            GUILayout.Space (2);
            using (new EditorGUILayout.HorizontalScope ()) {
                if (GUILayout.Button (Styles.ResetPositionLabel, GUILayout.Width (width))) {
                    Undo.RecordObjects (Selection.transforms, "Reset Position Selected Objects");
                    foreach (Transform t in Selection.transforms) {
                        t.localPosition = Vector3.zero;
                    }
                }
                if (GUILayout.Button (Styles.ResetRotationLabel, GUILayout.Width (width))) {
                    Undo.RecordObjects (Selection.transforms, "Reset Rotation Selected Objects");
                    foreach (Transform t in Selection.transforms) {
                        t.localRotation = Quaternion.Euler (Vector3.zero);
                    }
                }
                if (GUILayout.Button (Styles.ResetScaleLabel, GUILayout.Width (width))) {
                    Undo.RecordObjects (Selection.transforms, "Reset Scale Selected Objects");
                    foreach (Transform t in Selection.transforms) {
                        t.localScale = Vector3.one;
                    }
                }
            }
        }

        public override void ReloadPreviewInstances () {
            _defaultEditor.OnInspectorGUI ();
        }

        public override void DrawPreview (Rect previewArea) {
            _defaultEditor.HasPreviewGUI ();
        }

        public override void OnPreviewSettings () {
            _defaultEditor.OnPreviewSettings ();
        }

        public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height) {
            return _defaultEditor.RenderStaticPreview (assetPath, subAssets, width, height);
        }

        public override void OnPreviewGUI (Rect r, GUIStyle background) {
            _defaultEditor.OnPreviewGUI (r, background);
        }
    }
}