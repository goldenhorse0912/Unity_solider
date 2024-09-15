using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Vector3 Variable", menuName = "Variable/Create Vector3 Var"), ManageableData]
    public sealed class Vector3Variable : ScriptableObject {
        public Vector3 value;
    }
}