using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu, ManageableData]
    public class FieldOfView_SO : ScriptableObject {
        public float viewRadius;
        public float viewAngle;
        public LayerMask targetMask;
        public LayerMask obstacleMask;
    }
}