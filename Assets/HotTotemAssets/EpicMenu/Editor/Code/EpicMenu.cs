using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace HotTotemAssets.EpicMenu {
    public class EpicMenu : EditorWindow {
        static Texture2D background;
        static readonly int WindowHeight = 600, WindowWidth = 600;
        bool layoutSuccess;
        int borderSize = 1;
        static float MagnifierFactor = 1f;
        Vector2 CenterPoint;
        Ray ScreenToMouseRay;
        EpicMenuSettings Settings;
        GUIStyle SelectedTextStyle = new();
        GUIStyle UnselectedTextStyle = new();
        EpicMenuEntry SelectedEntry;
        Texture2D pieMenu;
        Texture2D controlDot;
        Texture2D unselectedEntry;
        Texture2D selectedEntry;
        bool Setup;

        public static EpicMenu Init (EpicMenuSettings settings, Vector2 spawnPosition, Ray screenToMouseRay) {
            var windows = (EpicMenu[])Resources.FindObjectsOfTypeAll (typeof (EpicMenu));
            foreach (var oldWindow in windows) {
                oldWindow.Close ();
            }
            var newWindow = ScriptableObject.CreateInstance<EpicMenu> ();
            newWindow.ScreenToMouseRay = screenToMouseRay;
            newWindow.Settings = settings;
            Color unselectedColor = EpicMenuStarter.GetUIColor (settings.Theme, false);
            newWindow.UnselectedTextStyle.normal.textColor = unselectedColor;
            newWindow.UnselectedTextStyle.wordWrap = true;
            newWindow.UnselectedTextStyle.alignment = TextAnchor.MiddleCenter;
            Color selectedColor = EpicMenuStarter.GetUIColor (settings.Theme, true);
            newWindow.SelectedTextStyle.normal.textColor = selectedColor;
            newWindow.SelectedTextStyle.wordWrap = true;
            newWindow.SelectedTextStyle.alignment = TextAnchor.MiddleCenter;
            newWindow.pieMenu = EpicMenuStarter.GetUIElement (settings.Theme, "PieMenu");
            newWindow.controlDot = EpicMenuStarter.GetUIElement (settings.Theme, "ControlDot");
            newWindow.unselectedEntry = EpicMenuStarter.GetUIElement (settings.Theme, "UnselectedOption");
            newWindow.selectedEntry = EpicMenuStarter.GetUIElement (settings.Theme, "SelectedOption");
            newWindow.position = new Rect (0, 0, 1, 1);
            EpicMenuOperators.blurredBG = null;
            Color borderColor;
            ColorUtility.TryParseHtmlString ("#fd4322", out borderColor);
            background = new Texture2D (1, 1, TextureFormat.RGBA32, false);
            background.SetPixel (0, 0, borderColor);
            background.Apply ();
#if UNITY_EDITOR_OSX
            newWindow.ShowUtility();
#else
            newWindow.ShowPopup ();
#endif
            return newWindow;
        }

        private void SetupWindow (Vector2 spawnPosition) {
            Setup = true;
            MagnifierFactor = EditorPrefs.GetFloat ("HotTotem.EpicMenu.DPIScaling", 1f);
            var screenShotStartX = 0f;
            var screenShotStartY = 0f;
            var centerPointY = 0f;
#if UNITY_EDITOR_OSX
            if (spawnPosition.x - WindowWidth / 2 > 0)
            {
                if (2 * (spawnPosition.x + WindowWidth / 2) > Screen.currentResolution.width)
                {
                    screenShotStartX = Screen.currentResolution.width / 2 - WindowWidth;
                }
                else
                {
                    screenShotStartX = (spawnPosition.x - WindowWidth / 2);
                }
            }
            if (spawnPosition.y - WindowHeight / 2 > 44)
            {
                if (2 * (spawnPosition.y + WindowHeight / 2) > Screen.currentResolution.height - 88)
                {
                    screenShotStartY = Screen.currentResolution.height / 2 - WindowHeight - 88;
                }
                else
                {
                    screenShotStartY = (spawnPosition.y - WindowHeight / 2);
                }
            }
            else
            {
                screenShotStartY += 44;
            }
            if (spawnPosition.y < WindowHeight / 4)
            {
                centerPointY = WindowHeight / 4;
                centerPointY += 44;
            }
            else
            {
                if (2 * (spawnPosition.y + WindowHeight / 4) > Screen.currentResolution.height - 88)
                {
                    centerPointY = 3 * WindowHeight / 4 ;
                }
                else
                {
                    centerPointY = spawnPosition.y - screenShotStartY;
                }
            }
#else
            if (spawnPosition.x - WindowWidth / 2 > 0) {
                if (MagnifierFactor * (spawnPosition.x + WindowWidth / 2) > Screen.currentResolution.width) {
                    screenShotStartX = Screen.currentResolution.width / MagnifierFactor - WindowWidth;
                } else {
                    screenShotStartX = (spawnPosition.x - WindowWidth / 2);
                }
            }
            if (spawnPosition.y - WindowHeight / 2 > 0) {
                if (MagnifierFactor * (spawnPosition.y + WindowHeight / 2) > Screen.currentResolution.height) {
                    screenShotStartY = Screen.currentResolution.height / MagnifierFactor - WindowHeight;
                } else {
                    screenShotStartY = (spawnPosition.y - WindowHeight / 2);
                }
            }
            if (spawnPosition.y < WindowHeight / 4) {
                centerPointY = WindowHeight / 4;
            } else {
                if (MagnifierFactor * (spawnPosition.y + WindowHeight / 4) > Screen.currentResolution.height) {
                    centerPointY = 3 * WindowHeight / 4;
                } else {
                    centerPointY = spawnPosition.y - screenShotStartY;
                }
            }
            borderSize = 1;
#endif
            CenterPoint = new Vector2 (screenShotStartX + (WindowWidth / 2), centerPointY);
            EpicMenuOperators.ScreenShot (new Rect ((screenShotStartX + borderSize) * MagnifierFactor, (screenShotStartY + borderSize) * MagnifierFactor, (WindowWidth - 2 * borderSize) * MagnifierFactor, (WindowHeight - 2 * borderSize) * MagnifierFactor));
            position = new Rect (screenShotStartX, screenShotStartY, WindowWidth, WindowHeight);
        }

        public void Accept () {
            if (SelectedEntry != null && SelectedEntry.Action != null) {
                SelectedEntry.Act (ScreenToMouseRay);
            }
            Close ();
        }

        void OnGUI () {
            if (!Setup) {
                SetupWindow (GUIUtility.GUIToScreenPoint (Event.current.mousePosition));
                layoutSuccess = false;
                return;
            }
            if (Event.current.type == EventType.Layout) {
                if (EpicMenuOperators.blurredBG == null) {
                    layoutSuccess = false;
                    return;
                } else {
                    layoutSuccess = true;
                }
            } else {
                if (Event.current.type == EventType.Repaint) {
                    if (!layoutSuccess) {
                        return;
                    }
                }
            }
            if (EpicMenuOperators.blurredBG != null) {
                GUI.DrawTexture (new Rect (0, 0, WindowWidth, WindowHeight), background, ScaleMode.StretchToFill);
                GUI.DrawTexture (new Rect (borderSize, borderSize, WindowWidth - borderSize * 2, WindowHeight - borderSize * 2), EpicMenuOperators.blurredBG, ScaleMode.ScaleToFit);
            }
            GUI.DrawTexture (new Rect (WindowWidth / 2 - 50, CenterPoint.y - 50, 100, 100), pieMenu, ScaleMode.ScaleToFit);
            var mouseToCenterOffset = (GUIUtility.GUIToScreenPoint (Event.current.mousePosition) - CenterPoint - new Vector2 (0, position.y));
            var dotDirection = mouseToCenterOffset;
            if (dotDirection.sqrMagnitude > 1600) {
                dotDirection = dotDirection.normalized * 40;
            }
            var selectedAngle = Mathf.PI * Vector2.Angle (Vector2.down, dotDirection) / 180;
            if (dotDirection.x < 0) {
                selectedAngle = 2 * Mathf.PI - selectedAngle;
            }
            selectedAngle -= Mathf.PI / 2;
            var dotPosition = new Vector2 (WindowWidth / 2 - 10, CenterPoint.y - 10) + dotDirection;
            GUI.DrawTexture (new Rect (dotPosition.x, dotPosition.y, 20, 20), controlDot, ScaleMode.ScaleToFit);
            foreach (var entry in Settings.EpicMenuEntries) {
                if (entry.IsEnabled) {
                    var entryAngle = (entry.Position - 2) * Mathf.PI / 4;
                    var entryPosition = new Vector2 (WindowWidth / 2, CenterPoint.y);
                    var addX = 0f;
                    var addY = 0f;
                    switch (entry.Position) {
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
                    if (mouseToCenterOffset.sqrMagnitude < 150) {
                        SelectedEntry = null;
                    }
                    if (((selectedAngle > entryAngle - Mathf.PI / 8 && (selectedAngle < entryAngle + Mathf.PI / 8)) || (selectedAngle > 2 * Mathf.PI + (entryAngle - Mathf.PI / 8) && selectedAngle < 2 * Mathf.PI + entryAngle + Mathf.PI / 8))
                     && mouseToCenterOffset.sqrMagnitude > 150) {
                        GUI.DrawTexture (new Rect (entryPosition.x, entryPosition.y, 128, 32), selectedEntry, ScaleMode.ScaleToFit);
                        SelectedEntry = entry;
                        GUI.Label (new Rect (entryPosition.x, entryPosition.y, 128, 32), entry.Title, SelectedTextStyle);
                        if (mouseToCenterOffset.sqrMagnitude > 16000 && SelectedEntry.Title.Contains ("Tools")) {
                            Accept ();
                        }
                    } else {
                        GUI.DrawTexture (new Rect (entryPosition.x, entryPosition.y, 128, 32), unselectedEntry, ScaleMode.ScaleToFit);
                        GUI.Label (new Rect (entryPosition.x, entryPosition.y, 128, 32), entry.Title, UnselectedTextStyle);
                        if (SelectedEntry == entry) {
                            SelectedEntry = null;
                        }
                    }
                }
            }
        }

        void Update () {
            Repaint ();
        }

        void OnDestroy () {
            EpicMenuOperators.blurredBG = null;
        }
    }

    [Serializable]
    public class EpicMenuSettings {
        public string DisplayName;
        public string FileName;
        public string Theme = "Default";
        public List<EpicMenuEntry> EpicMenuEntries;
        public bool IsActive;
        public EventModifiers Modifier;
        public Dictionary<KeyCode, bool> Keys;

        public bool Save (bool overwrite = false) {
            if (!overwrite && File.Exists (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/" + FileName)) {
                return false;
            } else {
                BinaryFormatter bf = new BinaryFormatter ();
                var ss = new SurrogateSelector ();
                ss.AddSurrogate (typeof (Dictionary<KeyCode, bool>),
                                 new StreamingContext (StreamingContextStates.All),
                                 new KeyCodeDictionarySerializationSurrogate ());
                // Associate the SurrogateSelector with the BinaryFormatter.
                bf.SurrogateSelector = ss;
                FileStream fs;
                fs = new FileStream (Application.dataPath + "/HotTotemAssets/EpicMenu/Editor/UserMenus/" + FileName, FileMode.Create);
                bf.Serialize (fs, this);
                fs.Close ();
                return true;
            }
        }

        public static EpicMenuSettings Load (string filePath) {
            var bf = new BinaryFormatter ();
            var ss = new SurrogateSelector ();
            ss.AddSurrogate (typeof (Dictionary<KeyCode, bool>),
                             new StreamingContext (StreamingContextStates.All),
                             new KeyCodeDictionarySerializationSurrogate ());
            // Associate the SurrogateSelector with the BinaryFormatter.
            bf.SurrogateSelector = ss;
            if (File.Exists (filePath)) {
                FileStream fs;
                fs = new FileStream (filePath, FileMode.Open);
                if (fs.Length == 0) {
                    fs.Close ();
                    return null;
                }
                try {
                    var obj = bf.Deserialize (fs);
                    fs.Close ();
                    return (EpicMenuSettings)obj;
                } catch (Exception e) {
                    Debug.LogError ("EpicMenu Error : " + e);
                }
            }
            return null;
        }
    }

    sealed class KeyCodeDictionarySerializationSurrogate : ISerializationSurrogate {
        public void GetObjectData (System.Object obj,
                                   SerializationInfo info, StreamingContext context) {
            var dict = (Dictionary<KeyCode, bool>)obj;
            info.AddValue ("count", dict.Count);
            int i = 0;
            foreach (var pair in dict) {
                var content = pair.Value;
                info.AddValue (i.ToString () + "key", pair.Key.ToString ());
                info.AddValue (i.ToString () + "value", content);
                i++;
            }
        }

        public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            var dict = new Dictionary<KeyCode, bool> ();
            var count = (int)info.GetValue ("count", typeof (int));
            for (int i = 0; i < count; i++) {
                var key = (KeyCode)Enum.Parse (typeof (KeyCode), info.GetString (i.ToString () + "key"));
                dict[key] = info.GetBoolean (i.ToString () + "value");
            }
            return dict;
        }
    }

    [Serializable]
    public class EpicMenuEntry {
        public int Position;
        public string Title;
        public string Action;
        public bool IsEnabled;

        public void Act (Ray screenToMouseRay) {
            Type t = Type.GetType (Action);
            EpicMenuAction o = (EpicMenuAction)Activator.CreateInstance (t);
            o.Action (screenToMouseRay);
        }
    }
}