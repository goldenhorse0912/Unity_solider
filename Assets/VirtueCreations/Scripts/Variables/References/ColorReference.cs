using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class ColorReference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        Color constantValue;
        [SerializeField]
        ColorVariable variable;

        public Color Value {
            get => useConstant ? constantValue : variable.value;
            set {
                if (useConstant)
                    constantValue = value;
                else
                    variable.value = value;
            }
        }
    }
}