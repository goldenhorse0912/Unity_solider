using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace VIRTUE {
    public static class Helper {
#if UNITY_EDITOR
        public static bool DisplayDialogue (string title, string message, string ok) => EditorUtility.DisplayDialog (title, message, ok);
        public static int DisplayDialogueComplex (string title, string message, string ok, string cancel, string alt) => EditorUtility.DisplayDialogComplex (title, message, ok, cancel, alt);
        public static void SetDirty (in Object targetObj) => EditorUtility.SetDirty (targetObj);
        public static void Pause () => EditorApplication.isPaused = true;
        public static void Resume () => EditorApplication.isPaused = false;

        public static void GenerateConstantsClass (string fullClassName, string dataType, List<KeyValuePair<object, object>> resourceKeys, bool alertWhenDone = false) {
            var constantsValues = string.Empty;
            var parts = fullClassName.Split ('.');
            var destinationFolder = "Assets/VirtueCreations/Scripts";

            // Construct the namespace based on provided className.
            var nameSpace = string.Empty;
            for (var i = 0; i < parts.Length - 1; i++) {
                if (nameSpace != string.Empty) nameSpace += ".";
                nameSpace += parts[i];
            }
            var hasNameSpace = nameSpace != string.Empty;
            var classTab = hasNameSpace ? "\t" : string.Empty;
            var memberTab = hasNameSpace ? "\t\t" : "\t";
            switch (dataType) {
                case "string":
                    foreach (var entry in resourceKeys) {
                        var key = MakeIdentifier ((string)entry.Key);
                        constantsValues += memberTab + $"public const {dataType} {key} = \"{entry.Value}\";\n";
                    }
                    constantsValues = constantsValues.Remove (constantsValues.Length - 1, 1);
                    break;

                case "int" or "byte":
                    foreach (var entry in resourceKeys) {
                        var key = MakeIdentifier ((string)entry.Key);
                        constantsValues += memberTab + $"public const {dataType} {key} = {entry.Value};\n";
                    }
                    constantsValues = constantsValues.Remove (constantsValues.Length - 1, 1);
                    break;

                default:
                    Debug.LogError ("Undefined datatype for const");
                    break;
            }

            // First read the template.
            var fileBody = ReadTemplate ("Template_Constants");

            // Write the namespace start.
            fileBody = hasNameSpace ? fileBody.Replace ("__NameSpaceStart__", "namespace " + nameSpace + " {") : fileBody.Replace ("__NameSpaceStart__", string.Empty);

            // Fill in appropriate class tab.
            fileBody = fileBody.Replace ("__ClassTab__", classTab);

            // Write the class name.
            var className = parts[^1];
            fileBody = fileBody.Replace ("__Class__", className);

            // Write the constants.
            fileBody = fileBody.Replace ("__Constant_Properties__", constantsValues);

            // Write the namespace end.
            fileBody = fileBody.Replace ("__NameSpaceEnd__", hasNameSpace ? "}" : string.Empty);

            // Write the file with the same name as the class.
            var filePath = destinationFolder + "/" + className + ".cs";
            WriteFile (filePath, fileBody);
            AssetDatabase.ImportAsset (filePath);
            if (alertWhenDone) EditorUtility.DisplayDialog ("Constants Class Generated", "Successfully created constants class at " + filePath, "OK");
        }

        public static void GenerateValueDropdownClass (string fullClassName, string listName, string dataType, List<KeyValuePair<object, object>> resourceKeys, bool alertWhenDone = false) {
            // var constantsValues = string.Format ("public static IEnumerable {0} = {", listName);
            var constantsValues = $"\t\tpublic static IEnumerable {listName}" + $" = new ValueDropdownList<{dataType}> " + "{";
            var parts = fullClassName.Split ('.');
            var destinationFolder = "Assets/VirtueCreations/Scripts";

            // Construct the namespace based on provided className.
            var nameSpace = string.Empty;
            for (var i = 0; i < parts.Length - 1; i++) {
                if (nameSpace != string.Empty) nameSpace += ".";
                nameSpace += parts[i];
            }
            var hasNameSpace = nameSpace != string.Empty;
            var classTab = hasNameSpace ? "\t" : string.Empty;
            var memberTab = hasNameSpace ? "\n\t\t\t" : "\n\t";
            switch (dataType) {
                case "string":
                    foreach (var entry in resourceKeys) {
                        var key = MakeIdentifier ((string)entry.Key);
                        constantsValues += memberTab + "{ " + $"\"{key.Remove (0, 1)}\", \"{entry.Value}\"" + " },";
                    }
                    break;

                case "int" or "byte":
                    foreach (var entry in resourceKeys) {
                        var key = MakeIdentifier ((string)entry.Key);
                        constantsValues += memberTab + "{ " + $"\"{key.Remove (0, 1)}\", {entry.Value}" + " },";
                    }
                    break;

                default:
                    Debug.LogError ("Undefined datatype for const");
                    break;
            }
            constantsValues += "\n\t\t};";

            // First read the template.
            var fileBody = ReadTemplate ("Template_Constants");

            // Write the namespace start.
            fileBody = fileBody.Insert (0, "using System.Collections;\nusing Sirenix.OdinInspector;\n\n");

            // Write the namespace start.
            fileBody = hasNameSpace ? fileBody.Replace ("__NameSpaceStart__", "namespace " + nameSpace + " {") : fileBody.Replace ("__NameSpaceStart__", string.Empty);

            // Fill in appropriate class tab.
            fileBody = fileBody.Replace ("__ClassTab__", classTab);

            // Write the class name.
            var className = parts[^1];
            fileBody = fileBody.Replace ("__Class__", className);

            // Write the constants.
            fileBody = fileBody.Replace ("__Constant_Properties__", constantsValues);

            // Write the namespace end.
            fileBody = fileBody.Replace ("__NameSpaceEnd__", hasNameSpace ? "}" : string.Empty);

            // Write the file with the same name as the class.
            var filePath = destinationFolder + "/" + className + ".cs";
            WriteFile (filePath, fileBody);
            AssetDatabase.ImportAsset (filePath);
            if (alertWhenDone) EditorUtility.DisplayDialog ("Constants Class Generated", "Successfully created constants class at " + filePath, "OK");
        }

        static string MakeIdentifier (string key) {
            var retId = string.Empty;
            if (string.IsNullOrEmpty (key)) return "_";
            var s = key.Trim ().Replace (' ', '_');
            s = s.Replace ('.', '_'); // Punctuations => Underscore _

            // Construct the identifier, ignoring all special characters like , + - * : /
            retId = s.Where (c => char.IsLetterOrDigit (c) || c == '_').Aggregate (retId, (current, c) => current + c);

            // Prefix leading numbers with underscore.
            if (char.IsDigit (retId[0])) retId = '_' + retId;
            return retId;
        }

        static void WriteFile (string path, string body) {
            using var wr = new StreamWriter (path, false);
            wr.Write (body);
        }

        static string ReadFile (string path) {
            if (!File.Exists (path)) return null;
            var sr = new StreamReader (path);
            var body = sr.ReadToEnd ();
            sr.Close ();
            return body;
        }

        static string ReadTemplate (string name) => ReadFile ($"Assets/VirtueCreations/Editor/Templates/{name}.txt");

        static readonly BuildTargetGroup[] BuildTargetGroups = {
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };
        public static List<string> GetDefinesList (BuildTargetGroup group) => new(PlayerSettings.GetScriptingDefineSymbolsForGroup (group).Split (';'));

        public static BuildTargetGroup GetCurrentBuildTargetGroup () {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup (target);
            return group;
        }

        public static void SetEnabled (string defineName, bool enable) {
            foreach (var group in BuildTargetGroups) {
                var defines = GetDefinesList (group);
                if (enable) {
                    if (defines.Contains (defineName)) return;
                    defines.Add (defineName);
                } else {
                    if (!defines.Contains (defineName)) return;
                    while (defines.Contains (defineName)) defines.Remove (defineName);
                }
                string definesString = string.Join (";", defines.ToArray ());
                PlayerSettings.SetScriptingDefineSymbolsForGroup (group, definesString);
            }
        }

        public static Texture GetTexture (string name) {
            var textures = Resources.FindObjectsOfTypeAll (typeof (Texture2D))
                                    .Where (t => t.name.Contains (name))
                                    .Cast<Texture2D> ().ToList ();
            return textures.Any () ? textures[0] : null;
        }
#endif

        static readonly string[] ShortNames = {
            "k",
            "M",
            "B",
            "T"
        };

        public static string Abbreviate (double value) {
            int nZeros = (int)Math.Log10 (value);
            int prefixIndex = nZeros / 3;
            if (prefixIndex > 0) {
                if (prefixIndex > 3) // Overflow..
                    prefixIndex = 3;
                double number = value / Math.Pow (10, prefixIndex * 3);
                return $"{number:0.##}{ShortNames[prefixIndex - 1]}";
            }
            return value.ToString ("0");
        }

        static Camera camera;
        public static Camera Camera => camera ? camera : camera = Camera.main;

        public static string Build (params string[] args) {
            var builder = new StringBuilder ();
            foreach (var word in args) {
                builder.Append (word);
            }
            return builder.ToString ();
        }

        public static bool InternetIsAvailable () => Application.internetReachability != NetworkReachability.NotReachable;

        static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

        public static WaitForSeconds WaitFor (in float duration) {
            if (WaitDictionary.TryGetValue (duration, out var wfs)) return wfs;
            WaitDictionary.Add (duration, new WaitForSeconds (duration));
            return WaitDictionary[duration];
        }

        public static float LoadingValue;

        public static float ClampToPer (in float currentAmount, in float maxAmount) => (maxAmount - currentAmount) / maxAmount;

        public static void SetBool (string key, bool value) => PlayerPrefs.SetInt (key, value ? 1 : 0);

        public static bool GetBool (string key, bool value) => PlayerPrefs.GetInt (key, value ? 1 : 0) == 1;
    }
}