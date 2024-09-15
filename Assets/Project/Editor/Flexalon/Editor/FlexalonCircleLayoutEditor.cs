using UnityEditor;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonCircleLayout)), CanEditMultipleObjects]
    public class FlexalonCircleLayoutEditor : FlexalonComponentEditor
    {
        private SerializedProperty _radius;
        private SerializedProperty _useWidth;
        private SerializedProperty _spiral;
        private SerializedProperty _spiralSpacing;
        private SerializedProperty _spacingType;
        private SerializedProperty _spacingDegrees;
        private SerializedProperty _radiusType;
        private SerializedProperty _radiusStep;
        private SerializedProperty _startAtDegrees;
        private SerializedProperty _rotate;
        private SerializedProperty _verticalAlign;

        [MenuItem("GameObject/Flexalon/Circle Layout")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonCircleLayout>("Circle Layout", command.context);
        }

        void OnEnable()
        {
            _radius = serializedObject.FindProperty("_radius");
            _useWidth = serializedObject.FindProperty("_useWidth");
            _spiral = serializedObject.FindProperty("_spiral");
            _spiralSpacing = serializedObject.FindProperty("_spiralSpacing");
            _spacingType = serializedObject.FindProperty("_spacingType");
            _spacingDegrees = serializedObject.FindProperty("_spacingDegrees");
            _radiusType = serializedObject.FindProperty("_radiusType");
            _radiusStep = serializedObject.FindProperty("_radiusStep");
            _startAtDegrees = serializedObject.FindProperty("_startAtDegrees");
            _rotate = serializedObject.FindProperty("_rotate");
            _verticalAlign = serializedObject.FindProperty("_verticalAlign");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ForceUpdateButton();

            if (!(target as FlexalonCircleLayout).UseWidth)
            {
                EditorGUILayout.PropertyField(_radius);
            }

            EditorGUILayout.PropertyField(_useWidth);
            EditorGUILayout.PropertyField(_spiral);

            if ((target as FlexalonCircleLayout).Spiral)
            {
                EditorGUILayout.PropertyField(_spiralSpacing);
            }

            EditorGUILayout.PropertyField(_spacingType);

            if ((target as FlexalonCircleLayout).SpacingType == FlexalonCircleLayout.SpacingOptions.Fixed)
            {
                EditorGUILayout.PropertyField(_spacingDegrees);
            }

            EditorGUILayout.PropertyField(_radiusType);
            if ((target as FlexalonCircleLayout).RadiusType != FlexalonCircleLayout.RadiusOptions.Constant)
            {
                EditorGUILayout.PropertyField(_radiusStep);
            }

            EditorGUILayout.PropertyField(_startAtDegrees);
            EditorGUILayout.PropertyField(_rotate);
            EditorGUILayout.PropertyField(_verticalAlign);

            ApplyModifiedProperties();
        }
    }
}