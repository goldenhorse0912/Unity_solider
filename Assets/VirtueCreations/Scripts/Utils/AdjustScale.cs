using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VIRTUE {
    public sealed class AdjustScale : MonoBehaviour {
#if UNITY_EDITOR
        [SerializeField]
        Transform _1, _2;
        [SerializeField]
        Transform _3, _4;

        [UnityEditor.MenuItem ("Developer/Create Objects")]
        static void CreateObjects () {
            if (GameObject.Find ("Adjust Scale Objects")) {
                return;
            }
            var parent = new GameObject ("Adjust Scale Objects");
            var obj1 = new GameObject ("_1") {
                transform = {
                    parent = parent.transform
                }
            };
            var obj2 = new GameObject ("_2") {
                transform = {
                    parent = parent.transform
                }
            };
            var obj3 = new GameObject ("_3") {
                transform = {
                    parent = parent.transform
                }
            };
            var obj4 = new GameObject ("_4") {
                transform = {
                    parent = parent.transform
                }
            };
            var iconContent = UnityEditor.EditorGUIUtility.IconContent ("sv_icon_dot4_pix16_gizmo");
            UnityEditor.EditorGUIUtility.SetIconForObject (obj1, (Texture2D)iconContent.image);
            UnityEditor.EditorGUIUtility.SetIconForObject (obj2, (Texture2D)iconContent.image);
            UnityEditor.EditorGUIUtility.SetIconForObject (obj3, (Texture2D)iconContent.image);
            UnityEditor.EditorGUIUtility.SetIconForObject (obj4, (Texture2D)iconContent.image);
        }

        void Reset () {
            _1 = GameObject.Find ("_1")?.transform;
            _2 = GameObject.Find ("_2")?.transform;
            _3 = GameObject.Find ("_3")?.transform;
            _4 = GameObject.Find ("_4")?.transform;
        }

        [ButtonGroup]
        void AdjustX () {
            var t = transform;
            var distanceX = Vector3.Distance (_1.position, _2.position);
            var newScale = t.localScale;
            newScale.x = distanceX;
            t.localScale = newScale;
        }

        [ButtonGroup]
        void AdjustZ () {
            var t = transform;
            var distanceZ = Vector3.Distance (_3.position, _4.position);
            var newScale = t.localScale;
            newScale.z = distanceZ;
            t.localScale = newScale;
        }

        [ButtonGroup]
        void AdjustY () {
            var t = transform;
            var distanceY = Vector3.Distance (_1.position, _2.position);
            var newScale = t.localScale;
            newScale.y = distanceY;
            t.localScale = newScale;
        }

        [ButtonGroup ("Last")]
        void Adjust () {
            var t = transform;
            var distanceX = Vector3.Distance (_1.position, _2.position);
            var distanceZ = Vector3.Distance (_3.position, _4.position);
            var newScale = t.localScale;
            newScale.x = distanceX;
            newScale.z = distanceZ;
            t.localScale = newScale;
        }

        [ButtonGroup ("Last")]
        void Angle () {
            Vector3 direction = _2.position - _1.position;
            float angle = Vector3.Angle (Vector3.forward, direction);
            Debug.Log (angle);
            EditorGUIUtility.systemCopyBuffer = angle.ToString ();
        }

        [ButtonGroup ("Last")]
        void Remove () {
            DestroyImmediate (this);
        }
#endif
    }
}