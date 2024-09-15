using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using C4Captain.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace C4Captain.Editor {
    public sealed class Snaps : OdinEditorWindow {
        public enum SnapType {
            None,
            Mesh,
            Collider,
            Group
        }

        public enum Center {
            Pivot,
            Bounds
        }

        [EnumToggleButtons]
        [HideLabel]
        public Center _center;

        [EnumToggleButtons]
        [HideLabel]
        public Space _coordinateSpace;

        [EnumToggleButtons]
        [HideLabel]
        public SnapType _snapType;

        [MenuItem ("Tools/Snaps/Open")]
        static void Open () {
            GetWindow<Snaps> ("Snaps");
        }

        protected override void OnGUI () {
            base.OnGUI ();
            //draw required fields
            // _center = (Center)EditorGUILayout.EnumPopup ("Center", _center);
            // _coordinateSpace = (Space)EditorGUILayout.EnumPopup ("Coordinate Space", _coordinateSpace);
            // _snapType = (SnapType)EditorGUILayout.EnumPopup ("Snap Type", _snapType);

            //caching the default color
            Color defaultColor = GUI.backgroundColor;

            //draw snap buttons
            EditorGUILayout.BeginHorizontal ();
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button ("-X", EditorStyles.miniButtonLeft)) Snap (Vector3.left, _center, _coordinateSpace, _snapType);
                if (GUILayout.Button ("+X", EditorStyles.miniButtonRight)) Snap (Vector3.right, _center, _coordinateSpace, _snapType);
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button ("-Y", EditorStyles.miniButtonLeft)) Snap (Vector3.down, _center, _coordinateSpace, _snapType);
                if (GUILayout.Button ("+Y", EditorStyles.miniButtonRight)) Snap (Vector3.up, _center, _coordinateSpace, _snapType);
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button ("-Z", EditorStyles.miniButtonLeft)) Snap (Vector3.back, _center, _coordinateSpace, _snapType);
                if (GUILayout.Button ("+Z", EditorStyles.miniButtonRight)) Snap (Vector3.forward, _center, _coordinateSpace, _snapType);
                GUI.backgroundColor = defaultColor;
            }
            EditorGUILayout.EndHorizontal ();
            GUI.enabled = false;
            EditorGUILayout.LabelField ("Selection count: " + Selection.objects.Length);
            GUI.enabled = true;
            if (GUI.changed) {
                EditorPrefs.SetInt ("Snap_Center", (int)_center);
                EditorPrefs.SetInt ("Snap_Space", (int)_coordinateSpace);
                EditorPrefs.SetInt ("Snap_SnapType", (int)_snapType);
            }
        }

        [Shortcut ("Snaps/Front", KeyCode.Keypad8, ShortcutModifiers.Alt)]
        static void Forward () => Snap (Vector3.forward, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        [Shortcut ("Snaps/Back", KeyCode.Keypad2, ShortcutModifiers.Alt)]
        static void Back () => Snap (Vector3.back, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        [Shortcut ("Snaps/Left", KeyCode.Keypad4, ShortcutModifiers.Alt)]
        static void Left () => Snap (Vector3.left, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        [Shortcut ("Snaps/Right", KeyCode.Keypad6, ShortcutModifiers.Alt)]
        static void Right () => Snap (Vector3.right, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        [Shortcut ("Snaps/Top", KeyCode.Keypad8, ShortcutModifiers.Alt | ShortcutModifiers.Control)]
        static void Up () => Snap (Vector3.up, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        [Shortcut ("Snaps/Bottom", KeyCode.Keypad2, ShortcutModifiers.Alt | ShortcutModifiers.Control)]
        static void Down () => Snap (Vector3.down, (Center) EditorPrefs.GetInt("Snap_Center"), (Space) EditorPrefs.GetInt("Snap_Space"), (SnapType) EditorPrefs.GetInt ("Snap_SnapType"));

        public static void Snap (Vector3 worldDir, Center center = Center.Pivot, Space space = Space.World, SnapType snapType = SnapType.None) {
            GameObject[] selected = Selection.gameObjects;
            int selectedLength = selected.Length;
            for (int i = 0; i < selectedLength; i++) {
                GameObject go = selected[i];

                //find the origin
                Vector3 origin = center == Center.Pivot ? go.transform.position : go.transform.GetBounds ().center;

                //find the direction
                Vector3 direction = space == Space.World ? worldDir : FindLocalDirection (worldDir, go);

                //casts multiple rays from origin to the desired direction
                //from which we will find the closest RaycastHit
                RaycastHit[] hits = Physics.RaycastAll (origin, direction);
                RaycastHit closestHit = new RaycastHit ();
                float closestDistance = Mathf.Infinity;

                //in case of GroupBounds we have to ignore childrens colliders
                //otherwise it will not work as intended
                if (snapType == SnapType.Group) {
                    List<RaycastHit> tempHits = new List<RaycastHit> (hits);
                    foreach (Transform tChild in go.GetComponentInChildren<Transform> ()) {
                        tempHits.RemoveAll (x => x.transform == tChild);
                    }
                    hits = tempHits.ToArray ();
                }

                //iterate through all the raycasts and find the closest RaycastHit
                int hitsLength = hits.Length;
                for (int j = 0; j < hitsLength; j++) {
                    float distance = Vector3.Distance (origin, hits[j].point);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestHit = hits[j];
                    }
                }
                switch (snapType) {
                    default:
                    case SnapType.None:
                        Undo.RecordObject (go.transform, "Position Snapped");
                        go.transform.position = closestHit.point;
                        break;

                    case SnapType.Mesh:
                        if (go.TryGetComponent (out MeshRenderer meshRenderer)) {
                            Undo.RecordObject (go.transform, "Position Snapped");
                            meshRenderer.transform.position += FindOffset (direction, closestHit.point, meshRenderer.bounds);
                        } else {
                            Debug.LogError ("Trying to snap with MeshBounds but no MeshRenderer found on : " + go.name, go);
                        }
                        break;

                    case SnapType.Collider:
                        if (go.TryGetComponent (out Collider collider)) {
                            Undo.RecordObject (go.transform, "Position Snapped");
                            collider.transform.position += FindOffset (direction, closestHit.point, collider.bounds);
                        } else {
                            Debug.LogError ("Trying to snap with ColliderBounds but no Collider found on : " + go.name, go);
                        }
                        break;

                    case SnapType.Group:
                        Undo.RecordObject (go.transform, "Position Snapped");
                        go.transform.position += FindOffset (direction, closestHit.point, go.transform.GetBounds ());
                        break;
                }
            }
        }

        static Vector3 FindLocalDirection (Vector3 original, GameObject go) {
            if (original == Vector3.right)
                return go.transform.right;
            else if (original == Vector3.left)
                return go.transform.right * -1f;
            else if (original == Vector3.up)
                return go.transform.up;
            else if (original == Vector3.down)
                return go.transform.up * -1f;
            else if (original == Vector3.forward)
                return go.transform.forward;
            else if (original == Vector3.back)
                return go.transform.forward * -1f;
            else
                return Vector3.zero;
        }

        static Vector3 FindOffset (Vector3 original, Vector3 hitPoint, Bounds bound) {
            if (original == Vector3.right)
                return new Vector3 (hitPoint.x - bound.max.x, 0, 0);
            else if (original == Vector3.left)
                return new Vector3 (hitPoint.x - bound.min.x, 0, 0);
            else if (original == Vector3.up)
                return new Vector3 (0, hitPoint.y - bound.max.y, 0);
            else if (original == Vector3.down)
                return new Vector3 (0, hitPoint.y - bound.min.y, 0);
            else if (original == Vector3.forward)
                return new Vector3 (0, 0, hitPoint.z - bound.max.z);
            else if (original == Vector3.back)
                return new Vector3 (0, 0, hitPoint.z - bound.min.z);
            else
                return Vector3.zero;
        }
    }
}