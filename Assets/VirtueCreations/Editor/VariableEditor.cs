using UnityEditor;
using UnityEngine;

namespace VIRTUE {
    internal class VariableEditor : Editor {
        protected void PaintInspectorGUI (string title) {
            serializedObject.Update ();
            MalbersEditor.DrawDescription (title);
            using (new EditorGUILayout.VerticalScope (MalbersEditor.StyleGray)) {
                using (new EditorGUILayout.VerticalScope (EditorStyles.helpBox)) {
                    EditorGUILayout.PropertyField (serializedObject.FindProperty ("value"), new GUIContent ("Value", "The current value"));
                    // EditorGUILayout.PropertyField (serializedObject.FindProperty ("Description"));
                }
            }
            serializedObject.ApplyModifiedProperties ();
        }
    }

    [CustomEditor (typeof (IntVariable)), CanEditMultipleObjects]
    internal class IntVarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("Int Variable");
        }
    }

    [CustomEditor (typeof (FloatVariable)), CanEditMultipleObjects]
    internal class FloatVarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("Float Variable");
        }
    }

    [CustomEditor (typeof (StringVariable)), CanEditMultipleObjects]
    internal class StringVarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("String Variable");
        }
    }

    [CustomEditor (typeof (Vector2Variable)), CanEditMultipleObjects]
    internal class Vector2VarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("Vector2 Variable");
        }
    }

    [CustomEditor (typeof (Vector3Variable)), CanEditMultipleObjects]
    internal class Vector3VarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("Vector3 Variable");
        }
    }

    [CustomEditor (typeof (ColorVariable)), CanEditMultipleObjects]
    internal class ColorVarEditor : VariableEditor {
        public override void OnInspectorGUI () {
            PaintInspectorGUI ("Color Variable");
        }
    }
}