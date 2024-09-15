using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class FloatReference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        float constantValue;
        [SerializeField]
        FloatVariable variable;

        public float Value {
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