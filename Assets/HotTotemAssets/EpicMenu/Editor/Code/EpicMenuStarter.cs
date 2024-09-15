using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HotTotemAssets.EpicMenu {
    [InitializeOnLoad]
    public class EpicMenuStarter : Editor {
        static Dictionary<EpicMenuSettings, EpicMenu> Combinations;
        static Dictionary<KeyCode, bool> PressedKeys;
        static Dictionary<string, Texture2D> UIElements;
        static Dictionary<string, Color> UIColors;
        public static Ray currentMouseRay;
        public static Vector2 currentMouseScreenPosition = new (0, 0);

        static EpicMenuStarter () {
            var sceneViews = SceneView.sceneViews;
            if (sceneViews.Count > 0) {
                ((SceneView)SceneView.sceneViews[0]).Focus ();
            }
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [InitializeOnLoadMethod]
        static void EditorInit () {
            if (!Directory.Exists (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus")) {
                Directory.CreateDirectory (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus");
            }
            if (!Directory.Exists (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/Custom")) {
                Directory.CreateDirectory (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/Custom");
            }
            UIElements = new Dictionary<string, Texture2D> ();
            UIColors = new Dictionary<string, Color> ();
            var availableThemes = Directory.GetDirectories (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes");
            AssetDatabase.Refresh ();
            foreach (var theme in availableThemes) {
                var themeName = Path.GetFileName (theme);
                UIElements[themeName + "PieMenu"] = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + themeName + "/PieMenu.png", typeof (Texture2D));
                UIElements[themeName + "ControlDot"] = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + themeName + "/ControlDot.png", typeof (Texture2D));
                UIElements[themeName + "UnselectedOption"] = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + themeName + "/UnselectedOption.png", typeof (Texture2D));
                UIElements[themeName + "SelectedOption"] = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + themeName + "/SelectedOption.png", typeof (Texture2D));
                StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/" + themeName + "/FontColors.txt");
                Color unselectedColor;
                ColorUtility.TryParseHtmlString (file.ReadLine (), out unselectedColor);
                UIColors[themeName + "unselected"] = unselectedColor;
                Color selectedColor;
                ColorUtility.TryParseHtmlString (file.ReadLine (), out selectedColor);
                UIColors[themeName + "selected"] = selectedColor;
                file.Close ();
            }
            if (UIElements.Any (x => x.Value == null)) {
                Debug.LogError ("Loading did not succeed");
            }
            var EpicConfigurations = new List<EpicMenuSettings> ();
            string[] files = Directory.GetFiles (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/", "*.epicMenu");
            foreach (var file in files) {
                EpicConfigurations.Add (EpicMenuSettings.Load (file));
            }
            RefreshCombinations (EpicConfigurations);
            PressedKeys = new Dictionary<KeyCode, bool> ();
            foreach (var key in Enum.GetValues (typeof (KeyCode))) {
                PressedKeys[(KeyCode)key] = false;
            }
            System.Reflection.FieldInfo info = typeof (EditorApplication).GetField ("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue (null);
            value -= EditorGlobalKeyPress;
            value += EditorGlobalKeyPress;
            info.SetValue (null, value);
#if UNITY_EDITOR_WIN
#if !UNITY_2018_2_OR_NEWER
            EditorPrefs.SetFloat("HotTotem.EpicMenu.DPIScaling", DPI.EpicMenuWindowsDPIHandler.GetDPI());
#endif
#else
            EditorPrefs.SetFloat("HotTotem.EpicMenu.DPIScaling", 1);
#endif
        }

        public static void RefreshCombinations (List<EpicMenuSettings> EpicConfigurations) {
            Combinations = new Dictionary<EpicMenuSettings, EpicMenu> ();
            foreach (var setting in EpicConfigurations) {
                Combinations[setting] = null;
            }
        }

        public static Texture2D GetUIElement (string theme, string elementName) {
            if (UIElements[theme + elementName] == null) {
                AssetDatabase.Refresh ();
                UIElements[theme + elementName] = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + theme + "/" + elementName + ".png", typeof (Texture2D));
            }
            return UIElements[theme + elementName];
        }

        public static Color GetUIColor (string theme, bool selected) {
            if (selected) {
                return UIColors[theme + "selected"];
            } else {
                return UIColors[theme + "unselected"];
            }
        }

        static void OnSceneGUI (SceneView sceneView) {
            currentMouseRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
            currentMouseScreenPosition = GUIUtility.GUIToScreenPoint (Event.current.mousePosition);
        }

        static void EditorGlobalKeyPress () {
            if (Event.current.isKey) {
                if (Event.current.type == EventType.KeyDown) {
                    PressedKeys[Event.current.keyCode] = true;
                } else if (Event.current.type == EventType.KeyUp) {
                    if (_menu != null) {
                        _menu.Accept ();
                    }
                    PressedKeys[Event.current.keyCode] = false;
                }
                for (int i = 0; i < Combinations.Count; i++) {
                    var combo = Combinations.ElementAt (i).Key;
                    if (Event.current.modifiers == combo.Modifier) {
                        foreach (var key in combo.Keys.Keys.ToList ()) {
                            if (Event.current.keyCode == key) {
                                if (Event.current.type == EventType.KeyDown) {
                                    var keyCombo = combo.Keys.Where (x => x.Value == true).ToList ();
                                    var comboPressed = true;
                                    foreach (var keyPress in keyCombo) {
                                        if (!PressedKeys[keyPress.Key]) {
                                            comboPressed = false;
                                        }
                                    }
                                    if (comboPressed && !combo.IsActive) {
                                        combo.IsActive = true;
                                        Combinations[combo] = EpicMenu.Init (combo, currentMouseScreenPosition, currentMouseRay);
                                        Event.current.Use ();
                                    }
                                    if (comboPressed && combo.IsActive) {
                                        Event.current.Use ();
                                    }
                                } else if (Event.current.type == EventType.KeyUp) {
                                    if (combo.IsActive && combo.Keys[key]) {
                                        combo.IsActive = false;
                                        if (Combinations[combo] != null) {
                                            Combinations[combo].Accept ();
                                        }
                                        Event.current.Use ();
                                    }
                                }
                            }
                        }
                    } else if (combo.IsActive) {
                        combo.IsActive = false;
                        if (Combinations[combo] != null) {
                            Combinations[combo].Accept ();
                        }
                        Event.current.Use ();
                    }
                }
            }
        }

        static EpicMenu _menu;

        public static void ActivateMenu (string menuName) {
            for (int i = 0; i < Combinations.Count; i++) {
                var combo = Combinations.ElementAt (i).Key;
                if (combo.DisplayName == menuName) {
                    if (!combo.IsActive) {
                        combo.IsActive = true;
                        _menu = EpicMenu.Init (combo, currentMouseScreenPosition, currentMouseRay);
                    }
                    break;
                }
            }
        }
    }
}