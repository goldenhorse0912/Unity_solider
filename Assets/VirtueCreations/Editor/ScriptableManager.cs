using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace VIRTUE {
    public sealed class ScriptableManager : OdinMenuEditorWindow {
        bool _isAsset;
        string _assetName = "New Asset";
        ScriptableObject _previewObject;
        const string SCRIPTABLE_DIR_PATH = "Assets/VirtueCreations/Scriptables/";
        static readonly Type[] TypesToDisplay = TypeCache.GetTypesWithAttribute<ManageableDataAttribute> ().OrderBy (m => m.Name).ToArray ();

        [MenuItem ("Tools/Scriptable Manager")]
        static void OpenWindow () {
            GetWindow<ScriptableManager> ().Show ();
        }

        static string GetMenuPathForType (Type t) {
            if (t == null || !TypesToDisplay.Contains (t)) return "";
            var typename = t.Name.Split ('`').First ().SplitPascalCase ();
            return GetMenuPathForType (t.BaseType) + "/" + typename;
        }

        protected override OdinMenuTree BuildMenuTree () {
            var tree = new OdinMenuTree (false) {
                Config = {
                    DrawSearchToolbar = true,
                    DrawScrollView = true
                }
            };
            tree.AddRange (TypesToDisplay.Where (x => !x.IsAbstract), GetMenuPathForType);
            tree.Selection.SelectionChanged += OnSelectionChanged;
            foreach (var type in TypesToDisplay) {
                tree.AddAllAssetsAtPath (GetMenuPathForType (type), SCRIPTABLE_DIR_PATH, type, true, true);
            }
            tree.SortMenuItemsByName ();
            return tree;
        }

        protected override IEnumerable<object> GetTargets () {
            yield return _previewObject;
        }

        protected override void DrawEditor (int index) {
            if (_previewObject) {
                if (_isAsset) {
                    EditorGUILayout.LabelField ("Path", AssetDatabase.GetAssetPath (_previewObject), EditorStyles.wordWrappedLabel);
                    SirenixEditorGUI.HorizontalLineSeparator (2);
                    base.DrawEditor (index);
                } else {
                    _assetName = EditorGUILayout.TextField ("AssetName", _assetName);
                    SirenixEditorGUI.HorizontalLineSeparator (2);
                    base.DrawEditor (index);
                    SirenixEditorGUI.HorizontalLineSeparator (2);
                    if (GUILayout.Button ("Create Asset", GUILayoutOptions.Height (30))) CreateAsset ();
                    GUILayout.FlexibleSpace ();
                }
                GUILayout.FlexibleSpace ();
            }
        }

        Type SelectedType {
            get {
                var m = MenuTree.Selection.LastOrDefault ();
                return m?.Value as Type;
            }
        }

        void OnSelectionChanged (SelectionChangedType obj) {
            if (_previewObject != null && !AssetDatabase.Contains (_previewObject)) DestroyImmediate (_previewObject);
            if (obj != SelectionChangedType.ItemAdded) return;
            CreatePreviewObject ();
        }

        void CreatePreviewObject () {
            var t = SelectedType;
            if (t is { IsAbstract: false }) {
                _isAsset = false;
                _previewObject = CreateInstance (t);
                _assetName = $"New {t.Name}";
            } else {
                _isAsset = true;
                _previewObject = MenuTree.Selection.SelectedValue as ScriptableObject;
            }
        }

        void CreateAsset () {
            var dirPath = $"{SCRIPTABLE_DIR_PATH}{_previewObject.GetType ().Name}s";
            if (!Directory.Exists (dirPath)) Directory.CreateDirectory (dirPath);
            var path = AssetDatabase.GenerateUniqueAssetPath ($"{dirPath}/{_assetName}.asset");
            AssetDatabase.CreateAsset (_previewObject, path);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
            CreatePreviewObject ();
        }

        protected override void OnBeginDrawEditors () {
            SirenixEditorGUI.BeginHorizontalToolbar ();
            {
                GUI.enabled = _isAsset;
                if (SirenixEditorGUI.ToolbarButton ("Delete Current")) DeleteAsset ();
                if (SirenixEditorGUI.ToolbarButton ("Ping")) SelectAsset ();
                GUILayout.FlexibleSpace ();
                GUI.enabled = true;
            }
            SirenixEditorGUI.EndHorizontalToolbar ();
        }

        void SelectAsset () {
            if (_previewObject == null) return;
            EditorGUIUtility.PingObject (_previewObject);
        }

        void DeleteAsset () {
            if (_previewObject == null) return;
            AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath (_previewObject));
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }
    }
}