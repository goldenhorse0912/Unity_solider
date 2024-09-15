using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class IntReference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        int constantValue;
        [SerializeField]
        IntVariable variable;

        public int Value {
            get => useConstant ? constantValue : variable.Value;
            set {
                if (useConstant)
                    constantValue = value;
                else
                    variable.Value = value;
            }
        }
    }
}