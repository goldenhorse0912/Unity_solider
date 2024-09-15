using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu, ManageableData]
    public class Troop_SO : ScriptableObject {
        public float attackRadius;
        public LayerMask visibleLayer;
        public LayerMask layerToIgnore;
        [ValueDropdown ("@VIRTUE.PooledObjectIds.PoolIds")]
        public string bulletId;
    }
}