using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class Vector3Reference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        Vector3 constantValue;
        [SerializeField]
        Vector3Variable variable;

        public Vector3 Value {
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