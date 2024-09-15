using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace VIRTUE {
    public sealed class Developer {
        [MenuItem ("CONTEXT/Animator/Enforce T-Pose")]
        static void TPose (MenuCommand command) {
            var animator = command.context as Animator;
            if (animator == null) {
                return;
            }
            if (!animator.avatar) return;
            SkeletonBone[] skeletonbones = animator.avatar?.humanDescription.skeleton;
            foreach (SkeletonBone sb in skeletonbones) {
                foreach (HumanBodyBones hbb in Enum.GetValues (typeof (HumanBodyBones))) {
                    if (hbb != HumanBodyBones.LastBone) {
                        Transform bone = animator.GetBoneTransform (hbb);
                        if (bone != null) {
                            if (sb.name == bone.name) {
                                if (hbb == HumanBodyBones.Hips) bone.localPosition = sb.position;
                                bone.localRotation = sb.rotation;
                                break;
                            }
                        }
                    }
                }
            }
        }

        [MenuItem ("CONTEXT/BoxCollider/Set Bounds")]
        static void SetBounds (MenuCommand command) {
            var boxCol = (BoxCollider)command.context;
            if (boxCol == null) {
                return;
            }
            var bounds = boxCol.gameObject.GetBounds ();
            boxCol.center = bounds.center;
            boxCol.size = bounds.size;
        }

        [MenuItem ("CONTEXT/CanvasScaler/Portrait")]
        static void Portrait (MenuCommand command) {
            var canvasScaler = (CanvasScaler)command.context;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2 (1080, 1920);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        [MenuItem ("CONTEXT/CanvasScaler/Landscape")]
        static void Landscape (MenuCommand command) {
            var canvasScaler = (CanvasScaler)command.context;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2 (1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        static bool FilterFromSelection (out Transform source, out Transform[] destination) {
            var selected = Selection.instanceIDs;
            if (selected.Any ()) {
                var srcGameObject = (GameObject)EditorUtility.InstanceIDToObject (selected.Last ());
                var dstGameObjects = new List<GameObject> ();
                for (int i = 0; i < selected.Length - 1; i++) {
                    dstGameObjects.Add ((GameObject)EditorUtility.InstanceIDToObject (selected[i]));
                }
                source = srcGameObject.transform;
                destination = dstGameObjects.Select (x => x.transform).ToArray ();
                return true;
            }
            source = null;
            destination = null;
            return false;
        }

        public static bool ValidateAlign () => Selection.transforms.Length >= 2;

        [MenuItem ("Align/Transform", false, 1)]
        public static void PerformTransformAlign () {
            if (FilterFromSelection (out var src, out var dst)) {
                Undo.RecordObjects (dst, "Align transforms with - " + src.name);
                for (int i = 0; i < dst.Length; i++) {
                    dst[i].SetPositionAndRotation (src.position, src.rotation);
                }
            }
        }

        [MenuItem ("Align/Rotation", false, 1)]
        public static void PerformRotationAlign () {
            if (FilterFromSelection (out var src, out var dst)) {
                Undo.RecordObjects (dst, "Align rotation with - " + src.name);
                foreach (var t in dst) {
                    t.rotation = src.rotation;
                }
            }
        }

        [MenuItem ("Align/Position", false, 1)]
        public static void PerformPositionAlign () {
            if (FilterFromSelection (out var src, out var dst)) {
                Undo.RecordObjects (dst, "Align position with - " + src.name);
                foreach (var t in dst) {
                    t.position = src.position;
                }
            }
        }

        [Shortcut ("Virtue Game Utilities/Tools/Unpack Prefab Completely", KeyCode.U)]
        static void Unpack () {
            // if (Selection.activeGameObject == null) return;
            /*var ans = EditorUtility.DisplayDialogComplex ("Unpack Options", "How do you want to unpack?", "Unpack Only", "Cancel", "Unpack Completely");
            var unpackMode = PrefabUnpackMode.Completely;
            switch (ans) {
                //Unpack Only
                case 0:
                    unpackMode = PrefabUnpackMode.OutermostRoot;
                    break;

                //Cancel
                case 1:
                    Debug.Log ("Unpack Operation Cancelled");
                    return;
            }*/
            foreach (var go in Selection.gameObjects) {
                Undo.RegisterCompleteObjectUndo (go, "Prefab Unpacked");
                PrefabUtility.UnpackPrefabInstance (go, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
            }
        }

        [MenuItem ("Assets/Import Model _i", false)]
        static void ImportModel () {
            foreach (var obj in Selection.objects) {
                var path = AssetDatabase.GetAssetPath (obj.GetInstanceID ());
                var modelImporter = AssetImporter.GetAtPath (path) as ModelImporter;
                if (modelImporter == null) continue;
                //model importer settings
                modelImporter.importVisibility = false;
                modelImporter.importCameras = false;
                modelImporter.importLights = false;
                modelImporter.sortHierarchyByName = false;
                if (path.IndexOf ("/VirtueCreations/FBX/Characters") != -1) {
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    if (path.Split ('/').Last ().Contains ("@")) {
                        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
                    } else {
                        modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                        modelImporter.importAnimation = false;
                    }
                } else {
                    modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                    modelImporter.importAnimation = false;
                }
                modelImporter.useSRGBMaterialColor = false;
                modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
                modelImporter.SaveAndReimport ();

                //create directories
                var materialsDir = path.Replace (path.Split ('/').Last (), "Materials");
                if (!Directory.Exists (materialsDir)) Directory.CreateDirectory (materialsDir);
                var texturesDir = path.Replace (path.Split ('/').Last (), "Textures");
                if (!Directory.Exists (texturesDir)) Directory.CreateDirectory (texturesDir);
                AssetDatabase.SaveAssets ();

                //extract textures
                modelImporter.ExtractTextures (texturesDir);
                if (Directory.GetFiles (texturesDir).Length.Equals (0)) {
                    AssetDatabase.DeleteAsset (texturesDir);
                    AssetDatabase.SaveAssets ();
                }
                modelImporter.SaveAndReimport ();

                //extract materials
                ExtractMaterials (path, materialsDir);

                //find materials and apply toony colors shader
                var materials = AssetDatabase.FindAssets ("t:Material", new[] {
                    materialsDir
                });
                foreach (var item in materials) {
                    var mat = AssetDatabase.LoadAssetAtPath<Material> (AssetDatabase.GUIDToAssetPath (item));
                    ApplyToony_Colors_Pro_2_Hybrid_Shader_2Shader (mat);
                }

                //find and resize textures
                var textures = AssetDatabase.FindAssets ("t:Texture", new[] { texturesDir });
                foreach (var item in textures) {
                    var assetPath = AssetDatabase.GUIDToAssetPath (item);
                    var textureImporter = AssetImporter.GetAtPath (assetPath) as TextureImporter;
                    if (textureImporter == null) continue;
                    var texture = AssetDatabase.LoadAssetAtPath<Texture> (assetPath);
                    textureImporter.mipmapEnabled = false;
                    textureImporter.textureCompression = TextureImporterCompression.CompressedLQ;
                    textureImporter.alphaIsTransparency = textureImporter.DoesSourceTextureHaveAlpha ();
                    textureImporter.maxTextureSize = Mathf.Max (texture.width, texture.height) >= 2048 ? 1024 : GetMaxSize (texture.width, texture.height);
                    textureImporter.SaveAndReimport ();
                }
            }
        }

        static void ExtractMaterials (string assetPath, string destinationPath) {
            HashSet<string> hashSet = new HashSet<string> ();
            IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath (assetPath) where x.GetType () == typeof (Material) select x;
            foreach (Object item in enumerable) {
                string path = Path.Combine (destinationPath, item.name) + ".mat";
                path = AssetDatabase.GenerateUniqueAssetPath (path);
                string value = AssetDatabase.ExtractAsset (item, path);
                if (string.IsNullOrEmpty (value)) {
                    hashSet.Add (assetPath);
                }
            }
            foreach (string item2 in hashSet) {
                AssetDatabase.WriteImportSettingsIfDirty (item2);
                AssetDatabase.ImportAsset (item2, ImportAssetOptions.ForceUpdate);
            }
        }

        static void ApplyToony_Colors_Pro_2_Hybrid_Shader_2Shader (Material mat) {
            if (mat.shader.name.Equals ("MK/Toon/URP/Standard/Simple")) {
                var texture = mat.GetTexture (ShaderParams.AlbedoMap);
                var color = mat.GetColor (ShaderParams.AlbedoColor);
                mat.shader = Shader.Find ("Toony Colors Pro 2/Hybrid Shader 2");
                if (texture != null) mat.SetTexture (ShaderParams.BaseMap, texture);
                mat.SetColor (ShaderParams.BaseColor, color);
            } else {
                var texture = mat.GetTexture (ShaderParams.MainTex);
                var color = mat.GetColor (ShaderParams.Color);
                mat.shader = Shader.Find ("Toony Colors Pro 2/Hybrid Shader 2");
                if (texture != null) mat.SetTexture (ShaderParams.BaseMap, texture);
                mat.SetColor (ShaderParams.BaseColor, color);
            }
            mat.SetColor (ShaderParams.SColor, new Color (0.8f, 0.8f, 0.8f));
            mat.SetFloat (ShaderParams.UseMobileMode, 1.0f);
            EditorUtility.SetDirty (mat);
        }

        static void ApplyMK_Toon_URP_Standard_SimpleShader (Material mat) {
            if (mat.shader.name.Equals ("Toony Colors Pro 2/Hybrid Shader 2")) {
                var texture = mat.GetTexture (ShaderParams.BaseMap);
                var color = mat.GetColor (ShaderParams.BaseColor);
                mat.shader = Shader.Find ("MK/Toon/URP/Standard/Simple");
                if (texture != null) mat.SetTexture (ShaderParams.AlbedoMap, texture);
                mat.SetColor (ShaderParams.AlbedoColor, color);
            } else {
                var texture = mat.GetTexture (ShaderParams.MainTex);
                var color = mat.GetColor (ShaderParams.Color);
                mat.shader = Shader.Find ("MK/Toon/URP/Standard/Simple");
                if (texture != null) mat.SetTexture (ShaderParams.AlbedoMap, texture);
                mat.SetColor (ShaderParams.AlbedoColor, color);
            }
            mat.SetColor (ShaderParams.GoochDarkColor, new Color (0.8f, 0.8f, 0.8f));
            mat.SetInt (ShaderParams.Specular, 0);
            EditorUtility.SetDirty (mat);
        }

        static readonly int[] TextureSizes = { 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        static int GetMaxSize (int width, int height) {
            var max = Mathf.Max (width, height);
            var textureSizesLength = TextureSizes.Length;
            for (var i = 0; i < textureSizesLength; i++) {
                if (max > TextureSizes[i]) continue;
                max = TextureSizes[i];
                break;
            }
            return max;
        }

        [MenuItem ("Tools/GPU/Enable _4")]
        static void EnableGPU () {
            foreach (var item in Selection.objects.Where (x => x.GetType () == typeof (Material))) {
                ToggleInstancing ((Material)item, true);
            }
        }

        [MenuItem ("Tools/GPU/Disable _5")]
        static void DisableGPU () {
            foreach (var item in Selection.objects.Where (x => x.GetType () == typeof (Material))) {
                ToggleInstancing ((Material)item, false);
            }
        }

        [MenuItem ("Tools/Shader/MK_Toon_URP_Standard_Simple")]
        static void ApplyMK_Toon_URP_Standard_SimpleShader () {
            foreach (var item in Selection.objects.Where (x => x.GetType () == typeof (Material))) {
                ApplyMK_Toon_URP_Standard_SimpleShader ((Material)item);
            }
        }

        [MenuItem ("Tools/Shader/Toony_Colors_Pro_2_Hybrid_Shader_2")]
        static void ApplyToony_Colors_Pro_2_Hybrid_Shader_2Shader () {
            foreach (var item in Selection.objects.Where (x => x.GetType () == typeof (Material))) {
                ApplyToony_Colors_Pro_2_Hybrid_Shader_2Shader ((Material)item);
            }
        }

        static void ToggleInstancing (Material mat, bool state) {
            mat.enableInstancing = state;
            Helper.SetDirty (mat);
        }

        [MenuItem ("GameObject/Create/At Selected P", false)]
        static void CreateEmptyAtSelectedP () {
            var selected = Selection.activeGameObject;
            if (selected == null) return;
            var newGameObject = new GameObject ($"Empty [{selected.name}]") {
                transform = {
                    position = selected.transform.position
                }
            };
            Undo.RegisterCreatedObjectUndo (newGameObject, "Empty Object Created");
            Selection.activeGameObject = newGameObject;
        }

        [MenuItem ("GameObject/Create/At Selected PR", false)]
        static void CreateEmptyAtSelectedPR () {
            var selected = Selection.activeGameObject;
            if (selected == null) return;
            var newGameObject = new GameObject ($"Empty [{selected.name}]") {
                transform = {
                    position = selected.transform.position,
                    rotation = selected.transform.rotation
                }
            };
            Undo.RegisterCreatedObjectUndo (newGameObject, "Empty Object Created");
            Selection.activeGameObject = newGameObject;
        }

        [MenuItem ("GameObject/Create/At Selected P", true)]
        [MenuItem ("GameObject/Create/At Selected PR", true)]
        static bool ValidateSelection () => Selection.activeGameObject != null;

        public const string DefineName = "DEVELOPER";

        [MenuItem ("Developer/Mode/Enable")]
        static void Enable () => Helper.SetEnabled (DefineName, true);

        [MenuItem ("Developer/Mode/Enable", true)]
        static bool EnableValidate () => !Helper.GetDefinesList (Helper.GetCurrentBuildTargetGroup ()).Contains (DefineName);

        [MenuItem ("Developer/Mode/Disable")]
        static void Disable () => Helper.SetEnabled (DefineName, false);

        [MenuItem ("Developer/Mode/Disable", true)]
        static bool DisableValidate () => Helper.GetDefinesList (Helper.GetCurrentBuildTargetGroup ()).Contains (DefineName);

        [MenuItem ("Developer/Select Unlockable Child")]
        static void SelectUnlockableChild () {
            List<Object> newObjects = new List<Object> ();
            foreach (var t in Selection.transforms) {
                if (t.childCount > 0) {
                    newObjects.Add (t.GetChild (0).gameObject);
                }
            }
            Selection.objects = newObjects.ToArray ();
        }
    }
}