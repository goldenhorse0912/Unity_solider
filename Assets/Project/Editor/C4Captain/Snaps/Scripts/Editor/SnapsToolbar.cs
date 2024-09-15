using System.Linq;
using C4Captain.Editor;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

namespace VIRTUE {
    [EditorToolbarElement (id, typeof (SceneView))]
    class SnapPosition : EditorToolbarDropdown {
        public const string id = "SnapsToolbar/Center";

        SnapPosition () {
            text = "Center";
            icon = EditorPrefs.GetInt ("Snap_Center", 0) switch {
                0 => EditorGUIUtility.FindTexture ("d_ToolHandlePivot"),
                1 => EditorGUIUtility.FindTexture ("d_ToolHandleCenter"),
                _ => icon
            };
            tooltip = "Select from where to snap";
            clicked += SelectOption;
        }

        void SelectOption () {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("Pivot"), EditorPrefs.GetInt ("Snap_Center", 0) == 0, Snap_Pivot);
            menu.AddItem (new GUIContent ("Center"), EditorPrefs.GetInt ("Snap_Center", 0) == 1, Snap_Center);
            menu.ShowAsContext ();
        }

        void Snap_Pivot () {
            icon = EditorGUIUtility.FindTexture ("d_ToolHandlePivot");
            EditorPrefs.SetInt ("Snap_Center", 0);
        }

        void Snap_Center () {
            icon = EditorGUIUtility.FindTexture ("d_ToolHandleCenter");
            EditorPrefs.SetInt ("Snap_Center", 1);
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class SnapSpace : EditorToolbarDropdown {
        public const string id = "SnapsToolbar/Space";

        SnapSpace () {
            text = "Space";
            icon = EditorPrefs.GetInt ("Snap_Space", 0) switch {
                0 => EditorGUIUtility.FindTexture ("d_ToolHandleGlobal"),
                1 => EditorGUIUtility.FindTexture ("d_ToolHandleLocal"),
                _ => icon
            };
            tooltip = "Select snap rotation";
            clicked += SelectOption;
        }

        void SelectOption () {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("Global"), EditorPrefs.GetInt ("Snap_Space", 0) == 0, Snap_World);
            menu.AddItem (new GUIContent ("Local"), EditorPrefs.GetInt ("Snap_Space", 0) == 1, Snap_Self);
            menu.ShowAsContext ();
        }

        void Snap_World () {
            icon = EditorGUIUtility.FindTexture ("d_ToolHandleGlobal");
            EditorPrefs.SetInt ("Snap_Space", 0);
        }

        void Snap_Self () {
            icon = EditorGUIUtility.FindTexture ("d_ToolHandleLocal");
            EditorPrefs.SetInt ("Snap_Space", 1);
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class SnapType : EditorToolbarDropdown {
        public const string id = "SnapsToolbar/Type";

        SnapType () {
            text = "Space";
            icon = EditorPrefs.GetInt ("Snap_SnapType", 0) switch {
                0 => null,
                1 => GetTexture ("d_Mesh Icon"),
                2 => GetTexture ("d_MeshCollider Icon"),
                3 => EditorGUIUtility.FindTexture ("d_GridLayoutGroup Icon"),
                _ => icon
            };
            tooltip = "Select snap type";
            clicked += SelectOption;
        }

        void SelectOption () {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("None"), EditorPrefs.GetInt ("Snap_SnapType", 0) == 0, Snap_None);
            menu.AddItem (new GUIContent ("Mesh"), EditorPrefs.GetInt ("Snap_SnapType", 0) == 1, Snap_Mesh);
            menu.AddItem (new GUIContent ("Collider"), EditorPrefs.GetInt ("Snap_SnapType", 0) == 2, Snap_Collider);
            menu.AddItem (new GUIContent ("Group"), EditorPrefs.GetInt ("Snap_SnapType", 0) == 3, Snap_Group);
            menu.ShowAsContext ();
        }

        void Snap_None () {
            icon = null;
            EditorPrefs.SetInt ("Snap_SnapType", 0);
        }

        void Snap_Mesh () {
            icon = GetTexture ("d_Mesh Icon");
            EditorPrefs.SetInt ("Snap_SnapType", 1);
        }

        void Snap_Collider () {
            icon = GetTexture ("d_MeshCollider Icon");
            EditorPrefs.SetInt ("Snap_SnapType", 2);
        }

        void Snap_Group () {
            icon = EditorGUIUtility.FindTexture ("d_GridLayoutGroup Icon");
            EditorPrefs.SetInt ("Snap_SnapType", 3);
        }

        static Texture2D GetTexture (string name) {
            var textures = Resources.FindObjectsOfTypeAll (typeof (Texture2D)).Where (t => t.name.Contains (name)).Cast<Texture2D> ().ToList ();
            return textures.Any () ? textures[0] : null;
        }
    }

    [EditorToolbarElement (id, typeof (SceneView))]
    class SnapButtons : EditorToolbarDropdown {
        public const string id = "SnapsToolbar/Buttons";
        Vector3 _snapDir;

        SnapButtons () {
            text = "Buttons";
            icon = null;
            tooltip = "Select where to snap";
            clicked += SelectOption;
        }

        void SelectOption () {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("Up"), _snapDir == Vector3.up, Snap_Up);
            menu.AddItem (new GUIContent ("Down"), _snapDir == Vector3.down, Snap_Down);
            menu.AddItem (new GUIContent ("Left"), _snapDir == Vector3.left, Snap_Left);
            menu.AddItem (new GUIContent ("Right"), _snapDir == Vector3.right, Snap_Right);
            menu.AddItem (new GUIContent ("Forward"), _snapDir == Vector3.forward, Snap_Forward);
            menu.AddItem (new GUIContent ("Back"), _snapDir == Vector3.back, Snap_Back);
            menu.ShowAsContext ();
        }

        void Snap_Up () {
            _snapDir = Vector3.up;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }

        void Snap_Down () {
            _snapDir = Vector3.down;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }

        void Snap_Left () {
            _snapDir = Vector3.left;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }

        void Snap_Right () {
            _snapDir = Vector3.right;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }

        void Snap_Forward () {
            _snapDir = Vector3.forward;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }

        void Snap_Back () {
            _snapDir = Vector3.back;
            Snaps.Snap (_snapDir, (Snaps.Center)EditorPrefs.GetInt ("Snap_Center"), (Space)EditorPrefs.GetInt ("Snap_Space"), (Snaps.SnapType)EditorPrefs.GetInt ("Snap_SnapType"));
        }
    }

    [Overlay (typeof (SceneView), "Snap Tools")]
    public class SnapToolbar : ToolbarOverlay {
        SnapToolbar () : base (SnapPosition.id, SnapSpace.id, SnapType.id, SnapButtons.id) { }
    }
}