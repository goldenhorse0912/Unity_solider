using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Vector2 Variable", menuName = "Variable/Create Vector2 Var"), ManageableData]
    public sealed class Vector2Variable : ScriptableObject {
        public Vector2 value;
    }
}