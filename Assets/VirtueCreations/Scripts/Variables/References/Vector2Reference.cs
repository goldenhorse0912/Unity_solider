using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class Vector2Reference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        Vector2 constantValue;
        [SerializeField]
        Vector2Variable variable;

        public Vector2 Value {
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