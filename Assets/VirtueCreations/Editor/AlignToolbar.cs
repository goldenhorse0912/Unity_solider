using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;

namespace VIRTUE {
    
    [EditorToolbarElement (id, typeof (SceneView))]
    class AlignTransform : EditorToolbarButton {
        public const string id = "AlignToolbar/Align Transform";

        public AlignTransform () {
            text = "Align Transform";
            icon = EditorGUIUtility.FindTexture ("TransformTool On");
            tooltip = "Align Transform";
            clicked += Align;
        }

        void Align () {
            if (Developer.ValidateAlign ()) {
                Developer.PerformTransformAlign ();
            }
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class AlignPosition : EditorToolbarButton {
        public const string id = "AlignToolbar/Align Position";

        public AlignPosition () {
            text = "Align Position";
            icon = EditorGUIUtility.FindTexture ("d_MoveTool On");
            tooltip = "Align Position";
            clicked += Align;
        }

        void Align () {
            if (Developer.ValidateAlign ()) {
                Developer.PerformPositionAlign ();
            }
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class AlignRotation : EditorToolbarButton {
        public const string id = "AlignToolbar/Align Rotation";

        public AlignRotation () {
            text = "Align Rotation";
            icon = EditorGUIUtility.FindTexture ("d_RotateTool On");
            tooltip = "Align Rotation";
            clicked += Align;
        }

        void Align () {
            if (Developer.ValidateAlign ()) {
                Developer.PerformRotationAlign ();
            }
        }
    }

    [Overlay (typeof (SceneView), "Align Tools")]
    public class AlignToolbar : ToolbarOverlay {
        AlignToolbar () : base (AlignTransform.id, AlignPosition.id, AlignRotation.id) { }
    }
}