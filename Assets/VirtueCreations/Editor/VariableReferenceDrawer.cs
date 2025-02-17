﻿using UnityEditor;
using UnityEngine;

namespace VIRTUE {
    [CustomPropertyDrawer (typeof (IntReference))]
    [CustomPropertyDrawer (typeof (FloatReference))]
    [CustomPropertyDrawer (typeof (StringReference))]
    [CustomPropertyDrawer (typeof (Vector2Reference))]
    [CustomPropertyDrawer (typeof (Vector3Reference))]
    [CustomPropertyDrawer (typeof (ColorReference))]
    internal sealed class VariableReferenceDrawer : PropertyDrawer {
        /// <summary>
        /// Options to display in the popup to select constant or variable.
        /// </summary>
        readonly string[] popupOptions =
        {
            "Use Constant",
            "Use Variable"
        };

        /// <summary> Cached style to use to draw the popup button. </summary>
        GUIStyle popupStyle;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            if (popupStyle == null) {
                popupStyle = new GUIStyle (GUI.skin.GetStyle ("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
            }
            label = EditorGUI.BeginProperty (position, label, property);
            position = EditorGUI.PrefixLabel (position, label);
            EditorGUI.BeginChangeCheck ();

            // Get properties
            SerializedProperty useConstant = property.FindPropertyRelative ("useConstant");
            SerializedProperty constantValue = property.FindPropertyRelative ("constantValue");
            SerializedProperty variable = property.FindPropertyRelative ("variable");

            // Calculate rect for configuration button
            Rect buttonRect = new Rect (position);
            buttonRect.yMin += popupStyle.margin.top;
            buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
            buttonRect.x -= 20;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            int result = EditorGUI.Popup (buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);
            useConstant.boolValue = (result == 0);
            EditorGUI.PropertyField (position, useConstant.boolValue ? constantValue : variable, GUIContent.none, false);
            if (EditorGUI.EndChangeCheck ()) property.serializedObject.ApplyModifiedProperties ();
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty ();
        }
    }
}