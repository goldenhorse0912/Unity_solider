using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Gradient Variable", menuName = "Variable/Create Gradient Var"), ManageableData]
    public class GradientVariable : ScriptableObject {
        public Gradient value;
    }
}