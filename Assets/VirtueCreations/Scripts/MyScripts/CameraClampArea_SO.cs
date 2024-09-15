using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu, ManageableData]
    public class CameraClampArea_SO : ScriptableObject {
        public float minX;
        public float maxX;
        public float minZ;
        public float maxZ;
    }
}