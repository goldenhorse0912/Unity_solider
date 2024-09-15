using UnityEngine;
using UnityEditor;

namespace Flexalon.Editor
{
    [InitializeOnLoad]
    internal class FlexalonMenu : EditorWindow
    {
        private static readonly string _website = "https://www.flexalon.com?utm_source=fxmenu";
        public static readonly string StoreLink = "https://assetstore.unity.com/packages/tools/utilities/flexalon-3d-layouts-230509?aid=1101lqSYn";
        private static readonly string _review = "https://assetstore.unity.com/packages/tools/utilities/flexalon-3d-layouts-230509#reviews";
        private static readonly string _discord = "https://discord.gg/VM9cWJ9rjH";
        private static readonly string _docs = "https://www.flexalon.com/docs?utm_source=fxmenu";
        private static readonly string _templates = "https://www.flexalon.com/templates?utm_source=fxmenu";
        private static readonly string _examples = "https://github.com/afarchy/flexalon-examples";
        private static readonly string _proxima = "https://www.unityproxima.com?utm_source=pxmenu";

        private static readonly string _showOnStartKey = "FlexalonMenu_ShowOnStart";
        private static readonly string _versionKey = "FlexalonMenu_Version";

        private GUIStyle _trialStyle;
        private GUIStyle _errorStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _versionStyle;
        private GUIStyle _boldStyle;
        private GUIStyle _proximaButtonStyle;

        private static ShowOnStart _showOnStart;
        private static readonly string[] _showOnStartOptions = {
            "Always", "On Update", "Never"
        };

        private Vector2 _scrollPosition;

        private enum ShowOnStart
        {
            Always,
            OnUpdate,
            Never
        }

        static FlexalonMenu()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            EditorApplication.update -= OnEditorUpdate;
            Initialize();
        }

        internal static void Initialize()
        {
            var shownKey = "FlexalonMenuShown";
            bool alreadyShown = SessionState.GetBool(shownKey, false);
            SessionState.SetBool(shownKey, true);

            var version = WindowUtil.GetVersion();
            var lastVersion = EditorPrefs.GetString(_versionKey, "0.0.0");
            var newVersion = version.CompareTo(lastVersion) > 0;
            if (newVersion)
            {
                FlexalonSurvey.ResetState();
                EditorPrefs.SetString(_versionKey, version);
                alreadyShown = false;
            }

            _showOnStart = (ShowOnStart)EditorPrefs.GetInt(_showOnStartKey, 0);
            bool showPref = _showOnStart == ShowOnStart.Always ||
                (_showOnStart == ShowOnStart.OnUpdate && newVersion);
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !alreadyShown && showPref)
            {
                StartScreen();
            }
            else
            {
                FlexalonTrial.UpdateRemainingDays();
            }

#if UNITY_WEB_REQUEST
            if (!FlexalonTrial.IsTrial && !EditorApplication.isPlayingOrWillChangePlaymode && FlexalonSurvey.ShouldAsk())
            {
                FlexalonSurvey.ShowSurvey();
            }
#endif
        }

        [MenuItem("Tools/Flexalon/Start Screen")]
        public static void StartScreen()
        {
            FlexalonTrial.UpdateRemainingDays();
            FlexalonMenu window = GetWindow<FlexalonMenu>(true, "Flexalon Start Screen", true);
            window.minSize = new Vector2(800, 600);
            window.maxSize = window.minSize;
            window.Show();
        }

        [MenuItem("Tools/Flexalon/Website")]
        public static void OpenStore()
        {
            Application.OpenURL(_website);
        }

        [MenuItem("Tools/Flexalon/Write a Review")]
        public static void OpenReview()
        {
            Application.OpenURL(_review);
        }

        [MenuItem("Tools/Flexalon/Support (Discord)")]
        public static void OpenSupport()
        {
            Application.OpenURL(_discord);
        }

        private void InitStyles()
        {
            if (_bodyStyle != null) return;

            _bodyStyle = new GUIStyle(EditorStyles.label);
            _bodyStyle.wordWrap = true;
            _bodyStyle.fontSize = 14;
            _bodyStyle.margin.left = 10;
            _bodyStyle.margin.top = 10;
            _bodyStyle.stretchWidth = false;

            _trialStyle = new GUIStyle(_bodyStyle);
            _trialStyle.fontStyle = FontStyle.Bold;
            _trialStyle.margin.top = 10;
            _trialStyle.normal.textColor = Color.yellow;

            _boldStyle = new GUIStyle(_bodyStyle);
            _boldStyle.fontStyle = FontStyle.Bold;
            _boldStyle.fontSize = 16;

            _errorStyle = new GUIStyle(_trialStyle);
            _errorStyle.normal.textColor = new Color(1, 0.2f, 0);

            _buttonStyle = new GUIStyle(_bodyStyle);
            _buttonStyle.fontSize = 14;
            _buttonStyle.margin.bottom = 5;
            _buttonStyle.padding.top = 5;
            _buttonStyle.padding.left = 10;
            _buttonStyle.padding.right = 10;
            _buttonStyle.padding.bottom = 5;
            _buttonStyle.hover.background = Texture2D.grayTexture;
            _buttonStyle.hover.textColor = Color.white;
            _buttonStyle.active.background = Texture2D.grayTexture;
            _buttonStyle.active.textColor = Color.white;
            _buttonStyle.focused.background = Texture2D.grayTexture;
            _buttonStyle.focused.textColor = Color.white;
            _buttonStyle.normal.background = Texture2D.grayTexture;
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.wordWrap = false;
            _buttonStyle.stretchWidth = false;

            _versionStyle = new GUIStyle(EditorStyles.label);
            _versionStyle.padding.right = 10;

            _proximaButtonStyle = new GUIStyle(_buttonStyle);
            _proximaButtonStyle.normal.background = Texture2D.blackTexture;
            _proximaButtonStyle.hover.background = Texture2D.blackTexture;
            _proximaButtonStyle.focused.background = Texture2D.blackTexture;
            _proximaButtonStyle.active.background = Texture2D.blackTexture;
            _proximaButtonStyle.padding.left = 0;
            _proximaButtonStyle.padding.right = 0;
            _proximaButtonStyle.padding.bottom = 0;
            _proximaButtonStyle.padding.top = 0;
            _proximaButtonStyle.margin.bottom = 10;

            WindowUtil.CenterOnEditor(this);
        }

        private void LinkButton(string label, string url, GUIStyle style = null, int width = 170)
        {
            if (style == null) style = _buttonStyle;
            var labelContent = new GUIContent(label);
            var position = GUILayoutUtility.GetRect(width, 35, style);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            if (GUI.Button(position, labelContent, style))
            {
                Application.OpenURL(url);
            }
        }

        private bool Button(string label, GUIStyle style = null, int width = 170)
        {
            if (style == null) style = _buttonStyle;
            var labelContent = new GUIContent(label);
            var position = GUILayoutUtility.GetRect(width, 35, style);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, labelContent, style);
        }

        private void Bullet(string text)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(" •", _bodyStyle);
            GUILayout.Label(text, _bodyStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void WhatsNew()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Update 3.2.1 (Hotfix)", _boldStyle);
            Bullet("Fixed Interactable custom editor not appearing correctly.");
            Bullet("Fixed Flexalon Draggable Max Objects property not working correctly.");
            Bullet("Fixed version checking to show the start screen on update.");
            EditorGUILayout.Space();
            GUILayout.Label("Update 3.2.0 (The XR Update)", _boldStyle);
            GUILayout.Label("Flexalon Version 3.2 support for VR interactions with integration for XR Interaction Toolkit and Oculus Interaction SDK.", _bodyStyle);
            EditorGUILayout.Space();
            GUILayout.Label("New Features:", _bodyStyle);
            Bullet("New 'Flexalon XR Input Provider' allows Flexalon Interactables to be used with interactables from XR Interaction Toolkit.");
            Bullet("New 'Flexalon Oculus Input Provider' allows Flexalon Interactables to be used with interactables from Oculus Interaction SDK.");
            Bullet("New 'Insert Radius' property for Flexalon Interactable allows you to specify how close a dragged object needs to be to a layout before it is inserted.");
            EditorGUILayout.Space();
            GUILayout.Label("Fixes and Changes:", _bodyStyle);
            Bullet("Flexalon Drag Target no longer creates a collider on itself. This prevents it from interfering with physics interactions. Instead, it uses a custom overlap detection function with the new Insert Radius property of Flexalon Interactable. Flexalon Interactable's generated placeholder no longer has a box collider for the same reason.");
            Bullet("Flexalon Interactable's generated placeholder no longer has a box collider for the same reason.");
            Bullet("If a Flexalon Interactable is dragged into two drag targets at once, it will now select the one with the nearest child.");
            Bullet("Flexalon Interactable local space restrictions no longer change to world space when leaving a drag target. This was causing objects to jump when leaving a drag target in some scenarios. Instead, the interactable continues to use the last drag target's local space until a new drag target is detected.");
            Bullet("Fixed a bug in which layouts were not always recomputed correctly when adding children from another layout.");
            Bullet("Fixed a bug in which layouts were not always recomputed correctly when a child without a Flexalon Component was deleted.");
            Bullet("Fixed a bug in which a Flexalon Object's offset would sometimes change unexpectedly.");
            Bullet("The Flexalon Result hidden component will no longer appear when a prefab is selected in the asset browser.");
            Bullet("The Game Object > Flexalon context menu will now add new layouts under the right-clicked gameObject.");
            EditorGUILayout.Space();
            GUILayout.Label("See CHANGELONG.md for all changes.", _bodyStyle);
        }

        private void OnGUI()
        {
            InitStyles();

            GUILayout.BeginHorizontal("In BigTitle", GUILayout.ExpandWidth(true));
            {
                WindowUtil.DrawFlexalonIcon(128);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Version: " + WindowUtil.GetVersion(), _versionStyle, GUILayout.ExpandHeight(true));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Resources", _boldStyle);
                    LinkButton("Discord Invite", _discord);
                    LinkButton("Documentation", _docs);
                    LinkButton("Templates", _templates);
                    LinkButton("More Examples", _examples);
                    LinkButton(FlexalonTrial.IsTrial ? "Reviews" : "Write a Review", _review);

#if UNITY_WEB_REQUEST
                    if (!FlexalonTrial.IsTrial && !FlexalonSurvey.Completed)
                    {
                        if (Button("Feedback"))
                        {
                            FlexalonSurvey.ShowSurvey();
                        }
                    }
#endif

                    GUILayout.FlexibleSpace();
                    GUILayout.Label("More Tools", _boldStyle);
                    if (WindowUtil.DrawProximaButton(128, _proximaButtonStyle))
                    {
                        Application.OpenURL(_proxima);
                    }

                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                }
                GUILayout.EndVertical();

                EditorGUILayout.Separator();

                GUILayout.BeginVertical();
                {
                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

                    GUILayout.Label("Thank you for using Flexalon!", _boldStyle);

                    EditorGUILayout.Space();

                    GUILayout.Label("You're invited to join the Discord community for support and feedback. Let me know how to make Flexalon better for you!", _bodyStyle);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        if (FlexalonTrial.IsTrial)
                        {
                            if (FlexalonTrial.RemainingDays > 0)
                            {
                                var day = FlexalonTrial.RemainingDays == 1 ? "day" : "days";
                                GUILayout.Label("You have " + FlexalonTrial.RemainingDays + " " + day + " left on your trial.", _trialStyle);
                            }
                            else
                            {
                                GUILayout.Label("Your trial has expired. Flexalon components will no longer update.", _errorStyle);
                            }

                            GUILayout.Label("You can upgrade the trial to a full license at any time without affecting your project.", _bodyStyle);

                            EditorGUILayout.Space();
                            LinkButton("Purchase Flexalon", StoreLink);
                        }
                        else
                        {
                            GUILayout.Label("If you enjoy Flexalon, please consider writing a review. It helps a ton!", _bodyStyle);
                        }

                        EditorGUILayout.Space();
                    }
                    GUILayout.EndVertical();

                    WhatsNew();

                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("In BigTitle", GUILayout.ExpandHeight(true));
            {
                GUILayout.Label("Tools/Flexalon/Start Screen");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Show On Start: ");
                var newShowOnStart = (ShowOnStart)EditorGUILayout.Popup((int)_showOnStart, _showOnStartOptions);
                if (_showOnStart != newShowOnStart)
                {
                    _showOnStart = newShowOnStart;
                    EditorPrefs.SetInt(_showOnStartKey, (int)_showOnStart);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}