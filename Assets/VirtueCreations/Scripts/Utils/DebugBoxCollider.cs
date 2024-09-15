using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    // [RequireComponent (typeof (BoxCollider))]
    public class DebugBoxCollider : MonoBehaviour {
#if UNITY_EDITOR
        [ReadOnly, SerializeField]
        BoxCollider col;

        [SerializeField]
        Color32 gizmoColor = Color.black;

        [HideIf ("HasBoxCollider")]
        [SerializeField]
        Vector3 center;

        [HideIf ("HasBoxCollider")]
        [SerializeField]
        Vector3 size;

        bool HasBoxCollider () => col != null;

        void Reset () {
            col = GetComponent<BoxCollider> ();
        }

        void OnDrawGizmos () {
            var t = transform;
            Gizmos.color = gizmoColor;
            var temp = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS (t.position, t.rotation, Vector3.one);
            var bounds = GetBounds ();
            var size = bounds.size;
            var scale = t.localScale;
            size.x *= scale.x;
            size.y *= scale.y;
            size.z *= scale.z;
            Gizmos.DrawWireCube (bounds.center, size);
            Gizmos.matrix = temp;
        }

        Bounds GetBounds () => col == null ? new Bounds (center, size) : new Bounds (col.center, col.size);
#endif
    }
}