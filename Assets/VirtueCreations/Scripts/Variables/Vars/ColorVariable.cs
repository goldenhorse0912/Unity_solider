using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Color Variable", menuName = "Variable/Create Color Var"), ManageableData]
    public sealed class ColorVariable : ScriptableObject {
        public Color32 value = Color.white;
    }
}