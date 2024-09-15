using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonObject)), CanEditMultipleObjects]
    public class FlexalonObjectEditor : FlexalonComponentEditor
    {
        private SerializedProperty _width;
        private SerializedProperty _widthType;
        private SerializedProperty _widthOfParent;
        private SerializedProperty _height;
        private SerializedProperty _heightType;
        private SerializedProperty _heightOfParent;
        private SerializedProperty _depth;
        private SerializedProperty _depthType;
        private SerializedProperty _depthOfParent;
        private SerializedProperty _offset;
        private SerializedProperty _rotation;
        private SerializedProperty _scale;
        private SerializedProperty _marginLeft;
        private SerializedProperty _marginRight;
        private SerializedProperty _marginTop;
        private SerializedProperty _marginBottom;
        private SerializedProperty _marginFront;
        private SerializedProperty _marginBack;
        private SerializedProperty _paddingLeft;
        private SerializedProperty _paddingRight;
        private SerializedProperty _paddingTop;
        private SerializedProperty _paddingBottom;
        private SerializedProperty _paddingFront;
        private SerializedProperty _paddingBack;

        private static bool _marginToggle;
        private static bool _paddingToggle;

        [MenuItem("GameObject/Flexalon/Flexalon Object")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonObject>("Flexalon Object", command.context);
        }

        void OnEnable()
        {
            _width = serializedObject.FindProperty("_width");
            _widthType = serializedObject.FindProperty("_widthType");
            _widthOfParent = serializedObject.FindProperty("_widthOfParent");
            _height = serializedObject.FindProperty("_height");
            _heightType = serializedObject.FindProperty("_heightType");
            _heightOfParent = serializedObject.FindProperty("_heightOfParent");
            _depth = serializedObject.FindProperty("_depth");
            _depthType = serializedObject.FindProperty("_depthType");
            _depthOfParent = serializedObject.FindProperty("_depthOfParent");
            _offset = serializedObject.FindProperty("_offset");
            _rotation = serializedObject.FindProperty("_rotation");
            _scale = serializedObject.FindProperty("_scale");
            _marginLeft = serializedObject.FindProperty("_marginLeft");
            _marginRight = serializedObject.FindProperty("_marginRight");
            _marginTop = serializedObject.FindProperty("_marginTop");
            _marginBottom = serializedObject.FindProperty("_marginBottom");
            _marginFront = serializedObject.FindProperty("_marginFront");
            _marginBack = serializedObject.FindProperty("_marginBack");
            _paddingLeft = serializedObject.FindProperty("_paddingLeft");
            _paddingRight = serializedObject.FindProperty("_paddingRight");
            _paddingTop = serializedObject.FindProperty("_paddingTop");
            _paddingBottom = serializedObject.FindProperty("_paddingBottom");
            _paddingFront = serializedObject.FindProperty("_paddingFront");
            _paddingBack = serializedObject.FindProperty("_paddingBack");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ForceUpdateButton();

            CreateSizeProperty(_widthType, _width, _widthOfParent);
            CreateSizeProperty(_heightType, _height, _heightOfParent);
            CreateSizeProperty(_depthType, _depth, _depthOfParent);
            EditorGUILayout.PropertyField(_offset);
            EditorGUILayout.PropertyField(_rotation);
            EditorGUILayout.PropertyField(_scale);

            _marginToggle = EditorGUILayout.Foldout(_marginToggle, "Margins");
            if (_marginToggle)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_marginLeft, new GUIContent("Left"));
                EditorGUILayout.PropertyField(_marginRight, new GUIContent("Right"));
                EditorGUILayout.PropertyField(_marginTop, new GUIContent("Top"));
                EditorGUILayout.PropertyField(_marginBottom, new GUIContent("Bottom"));
                EditorGUILayout.PropertyField(_marginFront, new GUIContent("Front"));
                EditorGUILayout.PropertyField(_marginBack, new GUIContent("Back"));
                EditorGUI.indentLevel--;
            }

            _paddingToggle = EditorGUILayout.Foldout(_paddingToggle, "Paddings");
            if (_paddingToggle)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_paddingLeft, new GUIContent("Left"));
                EditorGUILayout.PropertyField(_paddingRight, new GUIContent("Right"));
                EditorGUILayout.PropertyField(_paddingTop, new GUIContent("Top"));
                EditorGUILayout.PropertyField(_paddingBottom, new GUIContent("Bottom"));
                EditorGUILayout.PropertyField(_paddingFront, new GUIContent("Front"));
                EditorGUILayout.PropertyField(_paddingBack, new GUIContent("Back"));
                EditorGUI.indentLevel--;
            }

            ApplyModifiedProperties();
        }

        private void CreateSizeProperty(SerializedProperty typeProperty, SerializedProperty fixedProperty, SerializedProperty ofParentProperty)
        {
            EditorGUILayout.BeginHorizontal();
            bool showLabel = true;
            var labelContent = new GUIContent(fixedProperty.displayName);
            if (typeProperty.enumValueIndex == (int)SizeType.Fixed)
            {
                showLabel = false;
                EditorGUILayout.PropertyField(fixedProperty, labelContent, true);
            }
            else if (typeProperty.enumValueIndex == (int)SizeType.Fill)
            {
                showLabel = false;
                EditorGUILayout.PropertyField(ofParentProperty, labelContent, true);
            }

            EditorGUILayout.PropertyField(typeProperty, showLabel ? labelContent : GUIContent.none, true);
            EditorGUILayout.EndHorizontal();
        }

        void OnSceneGUI()
        {
            // Draw a box at the transforms position
            var script = target as FlexalonObject;
            var node = Flexalon.GetNode(script.gameObject);
            if (node == null || node.Result == null)
            {
                return;
            }

            var r = node.Result;

            if (node.Parent != null)
            {
                // Box used to layout this object, plus margins.
                Handles.color = new Color(1f, 1f, .2f, 1.0f);
                var layoutBoxScale = node.GetWorldBoxScale(false);
                var layoutRotation = script.transform.parent != null ? script.transform.parent.rotation * r.LayoutRotation : r.LayoutRotation;
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center + node.Margin.Center, r.RotatedAndScaledBounds.size + node.Margin.Size);

                // Box used to layout this object.
                Handles.color = new Color(.2f, 1f, .5f, 1.0f);
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center, r.RotatedAndScaledBounds.size);
            }

            // Box in which children are layed out. This is the box with handles on it.
            Handles.color = Color.cyan;
            var worldBoxScale = node.GetWorldBoxScale(true);
            Handles.matrix = Matrix4x4.TRS(node.GetWorldBoxPosition(worldBoxScale, false), script.transform.rotation, worldBoxScale);
            Handles.DrawWireCube(Vector3.zero, r.AdapterBounds.size);

            var id = 0;
            float result;
            if (script.WidthType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 0, script, out result))
                {
                    Record(script);
                    script.Width = result;
                    MarkDirty(script);
                }
            }

            if (script.HeightType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 1, script, out result))
                {
                    Record(script);
                    script.Height = result;
                    MarkDirty(script);
                }
            }

            if (script.DepthType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 2, script, out result))
                {
                    Record(script);
                    script.Depth = result;
                    MarkDirty(script);
                }
            }
        }

        private bool CreateSizeHandles(int id1, int id2, Vector3 size, int axis, FlexalonObject script, out float result)
        {
            bool changed = false;
            result = 0;

            if (CreateSizeHandleOnSide(id1, size, axis, 1, script, out float r1))
            {
                result = r1;
                changed = true;
            }

            if (CreateSizeHandleOnSide(id2, size, axis, -1, script, out float r2))
            {
                result = r2;
                changed = true;
            }

            return changed;
        }

        private bool CreateSizeHandleOnSide(int id, Vector3 size, int axis, int positive, FlexalonObject script, out float result)
        {
            var cid = GUIUtility.GetControlID(id, FocusType.Passive);
            var p = new Vector3();
            p[axis] = size[axis] / 2 * positive;
            EditorGUI.BeginChangeCheck();
#if UNITY_2022_1_OR_NEWER
            Vector3 newPos = Handles.FreeMoveHandle(cid, p, HandleUtility.GetHandleSize(p) * 0.2f, Vector3.one * 0.1f, Handles.SphereHandleCap);
#else
            Vector3 newPos = Handles.FreeMoveHandle(cid, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.2f, Vector3.one * 0.1f, Handles.SphereHandleCap);
#endif
            if (EditorGUI.EndChangeCheck())
            {
                result = newPos[axis] * 2 * positive;
                return true;
            }

            result = 0;
            return false;
        }
    }
}