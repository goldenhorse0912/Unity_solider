using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace VIRTUE {
    [EditorToolbarElement (id, typeof (SceneView))]
    class ResetPosition : EditorToolbarButton {
        public const string id = "ResetToolbar/Reset Position";

        public ResetPosition () {
            text = "Reset Position";
            icon = EditorGUIUtility.FindTexture ("MoveTool on");
            tooltip = "Reset Position";
            clicked += Reset;
        }

        void Reset () {
            var dst = Selection.transforms;
            if (dst.Any ()) {
                Undo.RecordObjects (dst, "Reset Position");
                foreach (var t in Selection.transforms) {
                    t.localPosition = Vector3.zero;
                }
            }
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class ResetRotation : EditorToolbarButton {
        public const string id = "ResetToolbar/Reset Rotation";

        public ResetRotation () {
            text = "Reset Rotation";
            icon = EditorGUIUtility.FindTexture ("RotateTool On");
            tooltip = "Reset Rotation";
            clicked += Reset;
        }

        void Reset () {
            var dst = Selection.transforms;
            if (dst.Any ()) {
                Undo.RecordObjects (dst, "Reset Rotation");
                foreach (var t in Selection.transforms) {
                    t.localRotation = Quaternion.Euler (Vector3.zero);
                }
            }
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class ResetScale : EditorToolbarButton {
        public const string id = "ResetToolbar/Reset Scale";

        public ResetScale () {
            text = "Reset Scale";
            icon = EditorGUIUtility.FindTexture ("ScaleTool On");
            tooltip = "Reset Scale";
            clicked += Reset;
        }

        void Reset () {
            var dst = Selection.transforms;
            if (dst.Any ()) {
                Undo.RecordObjects (dst, "Reset Scale");
                foreach (var t in dst) {
                    t.localScale = Vector3.one;
                }
            }
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class ResetTransform : EditorToolbarButton {
        public const string id = "ResetToolbar/Reset Transform";

        public ResetTransform () {
            text = "Reset Transform";
            icon = EditorGUIUtility.FindTexture ("TransformTool On");
            tooltip = "Reset Transform";
            clicked += Reset;
        }

        void Reset () {
            foreach (var t in Selection.transforms) {
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.Euler (Vector3.zero);
                t.localScale = Vector3.one;
            }
        }
    }

    [Overlay (typeof (SceneView), "Reset Tools")]
    public class ResetToolbar : ToolbarOverlay {
        ResetToolbar () : base (ResetPosition.id, ResetRotation.id, ResetScale.id, ResetTransform.id) { }
    }
}