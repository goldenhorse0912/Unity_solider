using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New String Variable", menuName = "Variable/Create String Var"), ManageableData]
    public sealed class StringVariable : ScriptableObject {
        public string value;
    }
}