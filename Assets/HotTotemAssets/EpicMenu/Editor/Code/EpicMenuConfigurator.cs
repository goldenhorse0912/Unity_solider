using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HotTotemAssets.EpicMenu {
    public class EpicMenuConfigurator : EditorWindow {
        public string[] epicMenuConfigurations = { "None", "Create New..." };
        string[] availableThemes;
        public List<EpicMenuSettings> EpicConfigurations;
        EpicMenuEntry SelectedEntry;
        GUIStyle UnselectedTextStyle = new();
        GUIStyle emptyButtonStyle = new();
        string[] DefaultActionClasses, DefaultActionNames;
        string[] CustomActionClasses, CustomActionNames;
        string[] SubMenus, SubMenuNames;
        public int index;
        int themeIndex;
        EpicMenuSettings selectedEpicMenuSettings;

        [MenuItem ("Window/HotTotemAssets/EpicMenu Settings %g")]
        static void Init () {
            // Get existing open window or if none, make a new one:
            EpicMenuConfigurator window = (EpicMenuConfigurator)GetWindow (typeof (EpicMenuConfigurator));
            window.position = new Rect (Screen.currentResolution.width / 3 - 300, Screen.currentResolution.height / 3 - 200, 600, 400);
            window.Show ();
        }

        void OnEnable () {
            titleContent = new GUIContent ("EpicMenu");
            var directories = Directory.GetDirectories (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/");
            availableThemes = new string[directories.Length];
            for (int i = 0; i < directories.Length; i++) {
                availableThemes[i] = Path.GetFileName (directories[i]);
            }
            RefreshEntries ();
        }

        void OnGUI () {
            GUILayout.Space (10);
            var newIndex = EditorGUILayout.Popup ("Manage your EpicMenus : ", index, epicMenuConfigurations);
            if (newIndex != index) {
                index = newIndex;
                selectedEpicMenuSettings = EpicConfigurations.FirstOrDefault (x => x.DisplayName == epicMenuConfigurations[index]);
                if (selectedEpicMenuSettings != null) {
                    StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/FontColors.txt");
                    Color unselectedColor;
                    ColorUtility.TryParseHtmlString (file.ReadLine (), out unselectedColor);
                    UnselectedTextStyle.normal.textColor = unselectedColor;
                    UnselectedTextStyle.wordWrap = true;
                    UnselectedTextStyle.alignment = TextAnchor.MiddleCenter;
                    file.Close ();
                    themeIndex = availableThemes.ToList ().IndexOf (selectedEpicMenuSettings.Theme);
                }
            }
            if (index == epicMenuConfigurations.Length - 1) {
                index = 0;
                EpicMenuNameInputDialog.ShowDialog (this);
            } else if (index == 0) {
                GUILayout.Space (150);
                GUILayout.BeginHorizontal ();
                GUILayout.Space (50);
                EditorGUILayout.LabelField ("No EpicMenu selected, pick one from the dropdown above or create a new one...");
                GUILayout.EndHorizontal ();
            } else {
                var newThemeIndex = EditorGUILayout.Popup ("Select a Theme : ", themeIndex, availableThemes);
                if (newThemeIndex != themeIndex) {
                    themeIndex = newThemeIndex;
                    selectedEpicMenuSettings.Theme = availableThemes[themeIndex];
                    selectedEpicMenuSettings.Save (true);
                    if (selectedEpicMenuSettings != null) {
                        StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/FontColors.txt");
                        Color unselectedColor;
                        ColorUtility.TryParseHtmlString (file.ReadLine (), out unselectedColor);
                        UnselectedTextStyle.normal.textColor = unselectedColor;
                        UnselectedTextStyle.wordWrap = true;
                        UnselectedTextStyle.alignment = TextAnchor.MiddleCenter;
                        file.Close ();
                        Repaint ();
                    }
                }
                GUILayout.Space (50);
                if (selectedEpicMenuSettings != null) {
                    Texture2D pieMenu = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/PieMenu.png", typeof (Texture2D));
                    Texture2D unselectedEntry = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/UnselectedOption.png", typeof (Texture2D));
                    GUI.DrawTexture (new Rect (250, 150, 100, 100), pieMenu, ScaleMode.ScaleToFit);
                    for (int i = 0; i < 8; i++) {
                        var entryAngle = (i - 2) * Mathf.PI / 4;
                        var entryPosition = new Vector2 (300, 200);
                        var addX = 0f;
                        var addY = 0f;
                        switch (i) {
                            case 0:
                            case 4:
                                addX = Mathf.Cos (entryAngle);
                                addY = Mathf.Sin (entryAngle);
                                entryPosition += new Vector2 (addX, addY) * 120;
                                entryPosition -= new Vector2 (64, 16);
                                break;

                            case 1:
                            case 5:
                                addX = Mathf.Cos (entryAngle + Mathf.PI / 12);
                                addY = Mathf.Sin (entryAngle + Mathf.PI / 12);
                                entryPosition += new Vector2 (addX, addY) * 120;
                                entryPosition -= new Vector2 (64, 16);
                                break;

                            case 3:
                            case 7:
                                addX = Mathf.Cos (entryAngle - Mathf.PI / 12);
                                addY = Mathf.Sin (entryAngle - Mathf.PI / 12);
                                entryPosition += new Vector2 (addX, addY) * 120;
                                entryPosition -= new Vector2 (64, 16);
                                break;

                            case 2:
                            case 6:
                                addX = Mathf.Cos (entryAngle);
                                addY = Mathf.Sin (entryAngle);
                                entryPosition += new Vector2 (addX, addY) * 160;
                                entryPosition -= new Vector2 (64, 16);
                                break;
                        }
                        if (GUI.Button (new Rect (entryPosition.x, entryPosition.y, 128, 32), unselectedEntry, emptyButtonStyle)) {
                            SelectedEntry = selectedEpicMenuSettings.EpicMenuEntries[i];
                            GenericMenu menu = new GenericMenu ();
                            menu.AddItem (new GUIContent ("None"), false, OnActionSelected, "None");
                            menu.AddSeparator ("");
                            for (int k = 0; k < DefaultActionNames.Length; k++) {
                                var action = DefaultActionNames[k];
                                menu.AddItem (new GUIContent ("Default Actions/" + action), false, OnActionSelected, "Default/" + DefaultActionClasses[k]);
                            }
                            menu.AddSeparator ("");
                            for (int k = 0; k < CustomActionNames.Length; k++) {
                                var action = CustomActionNames[k];
                                menu.AddItem (new GUIContent ("Custom Actions/" + action), false, OnActionSelected, "Custom/" + CustomActionClasses[k]);
                            }
                            menu.AddItem (new GUIContent ("Custom Actions/Create New..."), false, OnCreateNewAction);
                            menu.DropDown (new Rect (entryPosition.x, entryPosition.y, 128, 32));
                        }
                        GUI.Label (new Rect (entryPosition.x, entryPosition.y, 128, 32), selectedEpicMenuSettings.EpicMenuEntries[i].Title, UnselectedTextStyle);
                    }
                    var keyCombo = "None";
                    if (selectedEpicMenuSettings.Keys != null) {
                        if (selectedEpicMenuSettings.Keys.Any (x => x.Value == true)) {
                            keyCombo = "";
                            foreach (var keyValuePair in selectedEpicMenuSettings.Keys) {
                                if (keyValuePair.Value) {
                                    keyCombo += " + " + keyValuePair.Key.ToString ();
                                }
                            }
                            if (selectedEpicMenuSettings.Modifier == EventModifiers.None) {
                                keyCombo = keyCombo.Substring (3);
                            } else {
                                keyCombo = selectedEpicMenuSettings.Modifier.ToString () + keyCombo;
                            }
                        }
                    } else {
                        selectedEpicMenuSettings = EpicConfigurations.Where (x => x.DisplayName == epicMenuConfigurations[index]).FirstOrDefault ();
                        if (selectedEpicMenuSettings != null) {
                            StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/FontColors.txt");
                            Color unselectedColor;
                            ColorUtility.TryParseHtmlString (file.ReadLine (), out unselectedColor);
                            UnselectedTextStyle.normal.textColor = unselectedColor;
                            UnselectedTextStyle.wordWrap = true;
                            UnselectedTextStyle.alignment = TextAnchor.MiddleCenter;
                            file.Close ();
                            themeIndex = availableThemes.ToList ().IndexOf (selectedEpicMenuSettings.Theme);
                        }
                    }
                    GUI.Label (new Rect (15, 372, 60, 20), "Shortcut : ");
                    if (GUI.Button (new Rect (80, 370, 200, 20), keyCombo)) {
                        EpicMenuShortcutDialog.ShowDialog (selectedEpicMenuSettings);
                    }
                    if (GUI.Button (new Rect (480, 370, 100, 20), "Delete Menu")) {
                        if (EditorUtility.DisplayDialog ("Delete the menu ?", "Are you sure you want to delete the EpicMenu called " + selectedEpicMenuSettings.DisplayName, "Yes", "No")) {
                            FileUtil.DeleteFileOrDirectory (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/" + selectedEpicMenuSettings.FileName);
                            FileUtil.DeleteFileOrDirectory (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/" + selectedEpicMenuSettings.FileName + ".meta");
                            index = 0;
                            RefreshEntries ();
                        }
                    }
                } else {
                    if (index != 0 && index != epicMenuConfigurations.Length - 1) {
                        selectedEpicMenuSettings = EpicConfigurations.Where (x => x.DisplayName == epicMenuConfigurations[index]).FirstOrDefault ();
                        if (selectedEpicMenuSettings != null) {
                            StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Themes/" + selectedEpicMenuSettings.Theme + "/FontColors.txt");
                            Color unselectedColor;
                            ColorUtility.TryParseHtmlString (file.ReadLine (), out unselectedColor);
                            UnselectedTextStyle.normal.textColor = unselectedColor;
                            UnselectedTextStyle.wordWrap = true;
                            UnselectedTextStyle.alignment = TextAnchor.MiddleCenter;
                            file.Close ();
                            themeIndex = availableThemes.ToList ().IndexOf (selectedEpicMenuSettings.Theme);
                        }
                    }
                }
            }
        }

        public void OnActionSelected (object action) {
            var actionString = (string)action;
            if (actionString == "None") {
                SelectedEntry.Title = "None";
                SelectedEntry.IsEnabled = false;
            } else {
                SelectedEntry.Action = "HotTotemAssets.EpicMenu." + Path.GetFileName (actionString);
                StreamReader file = new StreamReader (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/" + actionString + ".cs");
                var nameline = "";
                var nameFound = false;
                while (!nameFound && (nameline = file.ReadLine ()) != null) {
                    if (nameline.Contains ("//DISPLAYNAME:")) {
                        nameline = nameline.Substring (14);
                        nameFound = true;
                    }
                }
                SelectedEntry.Title = nameline;
                SelectedEntry.IsEnabled = true;
            }
            selectedEpicMenuSettings.Save (true);
        }

        private void OnCreateNewAction () {
            EpicMenuActionNameInputDialog.ShowDialog (this);
        }

        internal void RefreshEntries () {
            EpicConfigurations = new List<EpicMenuSettings> ();
            string[] files = Directory.GetFiles (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/", "*.epicMenu");
            foreach (var file in files) {
                EpicConfigurations.Add (EpicMenuSettings.Load (file));
            }
            epicMenuConfigurations = new string[EpicConfigurations.Count + 2];
            epicMenuConfigurations[0] = "None";
            epicMenuConfigurations[epicMenuConfigurations.Length - 1] = "Create New...";
            for (int i = 0; i < EpicConfigurations.Count; i++) {
                epicMenuConfigurations[1 + i] = EpicConfigurations[i].DisplayName;
            }
            EpicMenuStarter.RefreshCombinations (EpicConfigurations);
            string[] defaultActions = Directory.GetFiles (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/Default", "*.cs");
            DefaultActionNames = new string[defaultActions.Length];
            DefaultActionClasses = new string[defaultActions.Length];
            for (int i = 0; i < defaultActions.Length; i++) {
                DefaultActionClasses[i] = Path.GetFileNameWithoutExtension (defaultActions[i]);
                StreamReader file = new StreamReader (defaultActions[i]);
                var nameline = "";
                var nameFound = false;
                while (!nameFound && (nameline = file.ReadLine ()) != null) {
                    if (nameline.Contains ("//DISPLAYNAME:")) {
                        nameline = nameline.Substring (14);
                        nameFound = true;
                    }
                }
                DefaultActionNames[i] = nameline;
            }
            string[] customActions = Directory.GetFiles (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/Custom", "*.cs");
            CustomActionNames = new string[customActions.Length];
            CustomActionClasses = new string[customActions.Length];
            for (int i = 0; i < customActions.Length; i++) {
                CustomActionClasses[i] = Path.GetFileNameWithoutExtension (customActions[i]);
                StreamReader file = new StreamReader (customActions[i]);
                var nameline = "";
                var nameFound = false;
                while (!nameFound && (nameline = file.ReadLine ()) != null) {
                    if (nameline.Contains ("//DISPLAYNAME:")) {
                        nameline = nameline.Substring (14);
                        nameFound = true;
                    }
                }
                CustomActionNames[i] = nameline;
            }
        }

        public static string GenerateClassName (string value) {
            string className = CultureInfo.CurrentCulture.TextInfo.ToTitleCase (value);
            bool isValid = Microsoft.CSharp.CSharpCodeProvider.CreateProvider ("C#").IsValidIdentifier (className);
            if (!isValid) {
                // File name contains invalid chars, remove them
                Regex regex = new Regex (@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                className = regex.Replace (className, "");

                // Class name doesn't begin with a letter, insert an underscore
                if (!char.IsLetter (className, 0)) {
                    className = className.Insert (0, "_");
                }
            }
            return className.Replace (" ", string.Empty);
        }
    }

    public class EpicMenuNameInputDialog : EditorWindow {
        string textInput = "";
        EpicMenuConfigurator parentInstance;

        public static void ShowDialog (EpicMenuConfigurator configurator) {
            EpicMenuNameInputDialog window = (EpicMenuNameInputDialog)EditorWindow.GetWindow (typeof (EpicMenuNameInputDialog));
            window.position = new Rect (Screen.width / 2, Screen.height / 2, 300, 50);
            window.titleContent = new GUIContent ("Name");
            window.parentInstance = configurator;
            window.Show ();
        }

        void OnGUI () {
            EditorGUILayout.LabelField ("Enter a Name for your Menu", EditorStyles.wordWrappedLabel);
            textInput = EditorGUILayout.TextField ("EpicMenu Name : ", textInput);
            GUILayout.Space (45);
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Done")) {
                var newEpicMenu = new EpicMenuSettings ();
                newEpicMenu.DisplayName = textInput;
                textInput = EpicMenuConfigurator.GenerateClassName (textInput);
                newEpicMenu.FileName = textInput + ".epicMenu";
                newEpicMenu.Theme = "Default";
                newEpicMenu.Keys = new Dictionary<KeyCode, bool> ();
                foreach (var key in Enum.GetValues (typeof (KeyCode))) {
                    newEpicMenu.Keys[(KeyCode)key] = false;
                }
                newEpicMenu.Modifier = EventModifiers.None;
                newEpicMenu.EpicMenuEntries = new List<EpicMenuEntry> () {
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 0
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 1
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 2
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 3
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 4
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 5
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 6
                    },
                    new() {
                        Title = "None",
                        IsEnabled = false,
                        Position = 7
                    }
                };
                if (newEpicMenu.Save ()) {
                    parentInstance.RefreshEntries ();
                    parentInstance.index = parentInstance.epicMenuConfigurations.ToList ().IndexOf (newEpicMenu.DisplayName);
                    Close ();
                } else {
                    EditorUtility.DisplayDialog ("Oh-oh!",
                                                 "An EpicMenu with the same name already exists, please chose another one or modify the existing one", "Alright");
                }
            }
            if (GUILayout.Button ("Cancel")) this.Close ();
            EditorGUILayout.EndHorizontal ();
        }
    }

    public class EpicMenuActionNameInputDialog : EditorWindow {
        string textInput = "";
        EpicMenuConfigurator parentInstance;

        public static void ShowDialog (EpicMenuConfigurator configurator) {
            EpicMenuActionNameInputDialog window = (EpicMenuActionNameInputDialog)GetWindow (typeof (EpicMenuActionNameInputDialog));
            window.position = new Rect (Screen.width / 2, Screen.height / 2, 300, 50);
            window.titleContent = new GUIContent ("Action Name");
            window.parentInstance = configurator;
            window.Show ();
        }

        void OnGUI () {
            EditorGUILayout.LabelField ("Enter a Name for your Action", EditorStyles.wordWrappedLabel);
            textInput = EditorGUILayout.TextField ("Action Name : ", textInput);
            GUILayout.Space (45);
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Done")) {
                var actionDisplayName = textInput;
                textInput = EpicMenuConfigurator.GenerateClassName (textInput);
                var actionFileName = textInput + ".cs";
                var destinationPath = Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Actions/Custom/" + actionFileName;
                if (!File.Exists (destinationPath)) {
                    var templateText = File.ReadAllText (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/Code/EpicMenuActionTemplate.cs");
                    templateText = templateText.Replace ("EpicMenuActionTemplateDisplayName", actionDisplayName);
                    templateText = templateText.Replace ("EpicMenuActionTemplate", textInput);
                    File.WriteAllText (destinationPath, templateText);
                    parentInstance.RefreshEntries ();
                    parentInstance.OnActionSelected ("Custom/" + textInput);
                    //UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (@destinationPath, 1);
                    Close ();
                } else {
                    EditorUtility.DisplayDialog ("Oh-oh!",
                                                 "An Action with the same name already exists, please chose another one or modify the existing one", "Alright");
                }
            }
            if (GUILayout.Button ("Cancel")) Close ();
            EditorGUILayout.EndHorizontal ();
        }
    }

    public class EpicMenuShortcutDialog : EditorWindow {
        Dictionary<KeyCode, bool> KeyCodes;
        EventModifiers Modifier;
        EpicMenuSettings SelectedSettings;
        Vector2 ModifierScrollPosition, KeyScrollPosition;

        public static void ShowDialog (EpicMenuSettings _selectedSettings) {
            EpicMenuShortcutDialog window = (EpicMenuShortcutDialog)GetWindow (typeof (EpicMenuShortcutDialog));
            window.position = new Rect (Screen.width / 2, Screen.height / 2, 500, 400);
            window.titleContent = new GUIContent ("Shortcuts");
            window.KeyCodes = _selectedSettings.Keys;
            window.Modifier = _selectedSettings.Modifier;
            window.SelectedSettings = _selectedSettings;
            window.ModifierScrollPosition = new Vector2 (0, 0);
            window.KeyScrollPosition = new Vector2 (0, 0);
            window.Show ();
        }

        void OnGUI () {
            EditorGUILayout.LabelField ("Select the keys you want to combine as shortcut for this EpicMenu", EditorStyles.wordWrappedLabel);
            var keyCodes = (KeyCode[])Enum.GetValues (typeof (KeyCode));
            var modifierCodes = (EventModifiers[])Enum.GetValues (typeof (EventModifiers));
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical ();
            EditorGUILayout.LabelField ("Select a modifier (only one selectable)");
            ModifierScrollPosition = EditorGUILayout.BeginScrollView (ModifierScrollPosition);
            for (int i = 0; i < modifierCodes.Length; i++) {
                var modifierSelected = Modifier == modifierCodes[i];
                var toggle = EditorGUILayout.ToggleLeft (modifierCodes[i].ToString (), modifierSelected);
                if (toggle) {
                    Modifier = modifierCodes[i];
                }
            }
            EditorGUILayout.EndScrollView ();
            EditorGUILayout.EndVertical ();
            var zIndex = keyCodes.ToList ().IndexOf (KeyCode.Z);
            EditorGUILayout.BeginVertical ();
            EditorGUILayout.LabelField ("Select key presses (unlimited amount)");
            KeyScrollPosition = EditorGUILayout.BeginScrollView (KeyScrollPosition);
            for (int i = 0; i < zIndex; i++) {
                KeyCodes[keyCodes[i]] = EditorGUILayout.ToggleLeft (keyCodes[i].ToString (), KeyCodes[keyCodes[i]]);
            }
            EditorGUILayout.EndScrollView ();
            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndHorizontal ();
            GUILayout.Space (20);
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Done")) {
                SelectedSettings.Keys = KeyCodes;
                SelectedSettings.Modifier = Modifier;
                SelectedSettings.Save (true);
                Close ();
            }
            if (GUILayout.Button ("Cancel")) Close ();
            EditorGUILayout.EndHorizontal ();
        }
    }
}